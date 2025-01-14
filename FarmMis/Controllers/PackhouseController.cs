using AAAErp.Constants;
using AAAErp.Controllers;
using AAAErp.IProvider;
using AAAErp.Models;
using AAAErp.Utilities;
using AAAErp.ViewModel;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmMis.Controllers
{
    public class PackhouseController : Controller
    {
        private InputValidator _validateService = new InputValidator();
        private readonly ILogger<HomeController> _logger;
        private readonly INotyfService _notyf;
        private readonly CoreDbContext _context;
        private MenuBuilder menuBuilder;
        public PackhouseController(CoreDbContext context, INotyfService notyf, ILogger<HomeController> logger)
        {
            _notyf = notyf;
            _context = context;
            _logger = logger;
            menuBuilder = new MenuBuilder(context);
        }

        [Authorize(Roles = "Packlist")]
        public async Task<IActionResult> Packlist(FilterUsersVm? filter)
        {
            filter.Page = filter?.Page ?? 1;
            var userId = HttpContext.User.FindFirst(StrValues.UserId)?.Value ?? "";
            menuBuilder.BuildMenus(this, userId, "Users");

            //filter.Page = filter.Page < 1 ? 1 : filter.Page;
            //var users = await GetUsers(filter);
            //int totalItems = users.Count();
            //var pager = new Pager(totalItems, filter.Page, StrValues.PageSize);
            //int skip = (filter.Page - 1) * StrValues.PageSize;
            //var data = users.Skip(skip).Take(pager.PageSize).ToList();

            //ViewBag.Pager = pager;
            //return View(new UserListFormVm
            //{
            //    List = data,
            //    Filter = filter
            //});

            return View();
        }
    }
}
