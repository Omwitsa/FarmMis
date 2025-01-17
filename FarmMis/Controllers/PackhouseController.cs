using FarmMis.Constants;
using FarmMis.IProvider;
using FarmMis.Models;
using FarmMis.Utilities;
using FarmMis.ViewModel;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;

namespace FarmMis.Controllers
{
    public class PackhouseController : Controller
    {
        private InputValidator _validateService = new InputValidator();
        private readonly ILogger<HomeController> _logger;
        private readonly INotyfService _notyf;
        private readonly CoreDbContext _context;
        private readonly IPortalProxy _portalProxy;
        private MenuBuilder menuBuilder;
        public PackhouseController(CoreDbContext context, INotyfService notyf, ILogger<HomeController> logger, IPortalProxy portalProxy)
        {
            _notyf = notyf;
            _context = context;
            _logger = logger;
            menuBuilder = new MenuBuilder(context);
            _portalProxy = portalProxy;
        }

        [Authorize(Roles = "Packlist")]
        public async Task<IActionResult> Packlist()
        {
            var userId = HttpContext.User.FindFirst(StrValues.UserId)?.Value ?? "";
            menuBuilder.BuildMenus(this, userId, "Packlist");
            await SetInitials();
            return View(new PacklistVm
            {
                Date = DateTime.Now,
            });
        }

        private async Task SetInitials()
        {
            var customers = await _context.Customers.ToListAsync();
            ViewBag.customers = new SelectList(customers, "VegId", "Name");
        }

        [Authorize(Roles = "Packlist")]
        [HttpPost]
        public async Task<IActionResult> Packlist([Bind("Date")] PacklistVm packlist)
        {
            var userId = HttpContext.User.FindFirst(StrValues.UserId)?.Value ?? "";
            menuBuilder.BuildMenus(this, userId, "Packlist");
            try
            {
                var setup = _context.SysSetup.FirstOrDefault();
                if (setup == null)
                {
                    _notyf.Error("Sorry, System settings not found");
                    return View(packlist);
                }
                setup.Farm = setup?.Farm ?? "";
                string date = packlist.Date.ToString("yyyyMMddHHmmss");
                var packlistResults = _portalProxy.GetPackList(setup.Farm, date).Result;
                var packlistobj = JsonConvert.DeserializeObject<dynamic>(packlistResults);

                foreach (var client in packlistobj.clients)
                {
                    var code = $"{client.code}";
                    if (!_context.Customers.Any(s => s.Code.ToUpper().Equals(code.ToUpper())))
                    {
                        await _context.Customers.AddAsync(new Customer
                        {
                            VegId = client.id,
                            Name = client.name,
                            Code = client.code,
                        });
                    }
                }

                foreach (var branch in packlistobj.branches)
                {
                    int id = branch.id;
                    if (!_context.Branches.Any(s => s.VegId == id))
                    {
                        await _context.Branches.AddAsync(new Branch
                        {
                            VegId = id,
                            Name = branch.name,
                            CustomerId = branch.customerId,
                        });
                    }
                }

                foreach (var product in packlistobj.products)
                {
                    int id = product.id;
                    if (!_context.Products.Any(s => s.VegId == id))
                    {
                        await _context.Products.AddAsync(new Product
                        {
                            VegId = id,
                            Name = product.name,
                            Code = product.code,
                            Type = product.type,
                            Category = product.category,
                        });
                    }
                }

                foreach (var dispatch in packlistobj.dispatch)
                {
                    int id = dispatch.dispatchheaderid;
                    var savedPacklist = _context.Packlists.Include(p => p.PacklistLines).FirstOrDefault(p => p.VegHeaderId == id);
                    if (savedPacklist == null)
                    {
                        var lines = new List<PacklistLine>();
                        foreach (var line in dispatch.line)
                        {
                            lines.Add(new PacklistLine
                            {
                                VegLineId = line.lineid,
                                VegHeaderId = line.headerid,
                                ProductId = line.productid,
                                BoxQty = line.boxqty,
                                Barcode = line.barcode,
                            });
                        }

                        await _context.Packlists.AddAsync(new Packlist
                        {
                            VegHeaderId = id,
                            DispatchDate = dispatch.dispatchdate,
                            CustomerId = dispatch.customerid,
                            CustomerName = dispatch.customername,
                            BranchId = dispatch.branchid,
                            BranchName = dispatch.branchname,
                            PacklistLines = lines,
                        });
                    }
                    else
                    {
                        foreach (var line in dispatch.line)
                        {
                            int lineId = line.lineid;
                            var packlistLine = savedPacklist.PacklistLines.FirstOrDefault(l => l.VegLineId == lineId);
                            if (packlistLine != null)
                                packlistLine.Barcode = line.barcode;
                        }
                    }
                }

                await _context.SaveChangesAsync();
                _notyf.Success("Synched successfully");
                await SetInitials();
                //return RedirectToAction("Packlist");
                return View(packlist);
            }
            catch (Exception ex)
            {
                _notyf.Error("Sorry, An error occurred");
                return View(packlist);
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetCustomerBranches(int customerId)
        {
            try
            {
                var branches = await _context.Branches.Where(b => b.CustomerId == customerId).ToListAsync();
                if(!branches.Any())
                    branches = await _context.Branches.Where(b => b.VegId == 0).ToListAsync();
                return Json(branches);
            }
            catch (Exception)
            {
                _notyf.Error("Sorry, An error occurred");
                return Json("");
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetCustomerProduct([FromBody] CustomerProduct customerProduct)
        {
            try
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(p => p.VegId == customerProduct.ClientId);
                if (customer == null)
                {
                    _notyf.Error("Sorry, Customer not found");
                    return Json("");
                }
                var branch = await _context.Branches.FirstOrDefaultAsync(p => p.VegId == customerProduct.BranchId);
                if (branch == null)
                {
                    _notyf.Error("Sorry, Customer branch not found");
                    return Json("");
                }

                var packlist = await _context.Packlists.Include(p => p.PacklistLines).FirstOrDefaultAsync(p => p.DispatchDate == customerProduct.Date 
                && p.CustomerId == customerProduct.ClientId && p.BranchId == customerProduct.BranchId);

                if(packlist == null)
                {
                    _notyf.Error($"Sorry, {customer.Name} {branch.Name} branch packlist not found on {customerProduct.Date}");
                    return Json("");
                }

                var line = packlist.PacklistLines.FirstOrDefault(p => p.Barcode.ToUpper().Equals(customerProduct.Barcode));
                if (line == null)
                {
                    _notyf.Error($"Sorry, {customer.Name} {branch.Name} branch didn't order the product on {customerProduct.Date}");
                    return Json("");
                }
                var product = await _context.Products.FirstOrDefaultAsync(p => p.VegId == line.ProductId);

                return Json(new
                {
                    line,
                    product
                });
            }
            catch (Exception)
            {
                _notyf.Error("Sorry, An error occurred");
                return Json("");
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetOrderedProducts([FromBody] CustomerProduct customerProduct)
        {
            try
            {
                if(customerProduct == null)
                    customerProduct = new CustomerProduct();

                var packlists = await _context.Packlists.Include(p => p.PacklistLines).Where(p => p.DispatchDate == customerProduct.Date).ToListAsync();
                if (customerProduct.ClientId != null)
                    packlists = packlists.Where(p => p.CustomerId == customerProduct.ClientId).ToList();
                if (customerProduct.BranchId != null)
                    packlists = packlists.Where(p => p.BranchId == customerProduct.BranchId).ToList();

                if(customerProduct.ClientId == null || customerProduct.BranchId == null)
                    packlists = packlists.Take(5).ToList();

                var products = await _context.Products.ToListAsync();
                var orderedProducts = new List<dynamic>();
                foreach (var pack in packlists)
                {
                    foreach (var line in pack.PacklistLines)
                    {
                        var product = products.FirstOrDefault(p => p.VegId == line.ProductId);
                        orderedProducts.Add(new
                        {
                            product = product.Name,
                            boxQty = line.BoxQty,
                            scanQty = line.ScanQty,
                            barcode = line.Barcode,
                        });
                    }
                }

                return Json(new
                {
                    orderedProducts
                });
            }
            catch (Exception)
            {
                _notyf.Error("Sorry, An error occurred");
                return Json("");
            }
        }

    }
}
