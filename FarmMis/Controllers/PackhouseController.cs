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

            var customers = await _context.Customers.Select(s => s.Name).ToListAsync();
            ViewBag.customers = new SelectList(customers);

            return View(new PacklistVm
            {
                Date = DateTime.Now,
            });
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
                return RedirectToAction("Packlist");
            }
            catch (Exception ex)
            {
                _notyf.Error("Sorry, An error occurred");
                return View(packlist);
            }
        }
    }
}
