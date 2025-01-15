using FarmMis.Constants;
using FarmMis.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using FarmMis.ViewModel;
using FarmMis.Utilities;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using FarmMis.IProvider;

namespace FarmMis.Controllers
{
    public class HomeController : Controller
    {
		private InputValidator _validateService = new InputValidator();
        private readonly ILogger<HomeController> _logger;
        private readonly INotyfService _notyf;
		private readonly CoreDbContext _context;
        private MenuBuilder menuBuilder;
        private readonly ICronJobProvider _jobProvider;

        public HomeController(CoreDbContext context,INotyfService notyf, ICronJobProvider jobProvider, ILogger<HomeController> logger)
        {
			_notyf = notyf;
			_context = context;
            _logger = logger;
            _jobProvider = jobProvider;
            menuBuilder = new MenuBuilder(context);
        }
		[Authorize]
		public IActionResult Index()
        {
            var userId = HttpContext.User.FindFirst(StrValues.UserId)?.Value ?? "";
            menuBuilder.BuildMenus(this, userId,"");
            return View();
        } 

        public IActionResult Login(string returnUrl)
		{
            return View();
		}

        public JsonResult TestMail()
        {
            //_jobProvider.SendBirtdayMails();
            return Json("");
        }

        public IActionResult Denied()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(string userID, string password, string returnUrl)
		{
			ViewData["ReturnUrl"] = returnUrl;
			var userInput = new List<Tuple<string, string, InputDataType>>
			{
				Tuple.Create("username", userID, InputDataType.Default),
				Tuple.Create("password", password, InputDataType.Password),
			};

			var validUserInputs = _validateService.Validate(userInput);
			if (!validUserInputs.Success)
			{
				_notyf.Error(validUserInputs.Message);
				return View();
			}

			var dbUser = await ValidateUser(userID, password);
			if (dbUser == null)
			{
				_notyf.Error("Sorry, Invalid user credentials");
				return View();
			}

			if (!dbUser.Status)
			{
				_notyf.Error("Sorry, Your account has been temporarily disabled. Kindly contact admin");
				return View();
			}
          
            // Create Claims 
            var claims = new List<Claim>();
			claims.Add(new Claim(ClaimTypes.Name, dbUser.Names));
			claims.Add(new Claim(StrValues.UserId, dbUser.UserID));

            var assignedGroups = dbUser.AssignedGroups.Select(g => g.Group.ToUpper()).ToList();
            var userGroups = _context.UserGroups.Include(g => g.UserPrivileges).Where(g => assignedGroups.Contains(g.Name.ToUpper())).ToList();

            var userPrivileges = new List<UserPrivilege>();
            foreach (var userGroup in userGroups)
                foreach (var privilege in userGroup.UserPrivileges)
                    if (privilege.AccessRight != AccessRight.Hidden)
                    {
                        var code = Decryptor.Decrypt(privilege.PrivilegeCode);
						var currentPrivilege = ArrValues.Privileges.FirstOrDefault(p => p.Code == code);

						if(currentPrivilege != null)
                            claims.Add(new Claim(ClaimTypes.Role, currentPrivilege.Name));
                    }

            // Create Identity
            var claimsIdentity = new ClaimsIdentity(claims,
				CookieAuthenticationDefaults.AuthenticationScheme);

			// Create Principal 

			var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

			var authProperties = new AuthenticationProperties
			{
				ExpiresUtc = DateTime.Now.AddMinutes(30),
				//IsPersistent = login.RememberLogin
			};
			// Sign In
			await HttpContext.SignInAsync(claimsPrincipal, authProperties);

			_notyf.Success("Logged in successfully");
			// Redirect
			if (!string.IsNullOrEmpty(returnUrl))
				return Redirect(returnUrl);
			else
				return RedirectToAction("Index");
		}

		public async Task<User> ValidateUser(string userID, string password)
		{
			password = Decryptor.Encrypt(password);
			var dbUser = await _context.Users.Include(u => u.AssignedGroups)
				.FirstOrDefaultAsync(u => u.UserID.ToUpper().Equals(userID.ToUpper()) && u.Password == password);

			return dbUser;
		}

		public IActionResult Privacy()
        {
            return View();
        }

        //[Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}