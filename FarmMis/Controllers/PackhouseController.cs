using FarmMis.Constants;
using FarmMis.Controllers;
using FarmMis.IProvider;
using FarmMis.Models;
using FarmMis.Utilities;
using FarmMis.ViewModel;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using Microsoft.ReportingServices.Interfaces;

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
        public async Task<IActionResult> Packlist(FilterUsersVm? filter)
        {
            filter.Page = filter?.Page ?? 1;
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
        public IActionResult Packlist([Bind("Date")] PacklistVm packlist)
        {
            
            var userId = HttpContext.User.FindFirst(StrValues.UserId)?.Value ?? "";
            menuBuilder.BuildMenus(this, userId, "Packlist");

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

            var val = packlistobj.clients;
            //ViewBag.appliedLeaves = jdata.Data.applications;
            return View();
        }
    }
}
