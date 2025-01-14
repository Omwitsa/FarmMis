using AspNetCoreHero.ToastNotification.Abstractions;
using ClosedXML.Excel;
using AAAErp.Constants;
using AAAErp.IProvider;
using AAAErp.Models;
using AAAErp.Utilities;
using AAAErp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;

namespace AAAErp.Controllers
{
    public class AdminController : Controller
    {
        private readonly INotyfService _notyf;
        private readonly CoreDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IEmailProvider _emailProvider;
        private MenuBuilder menuBuilder;
        public AdminController(CoreDbContext context, INotyfService notyf, IWebHostEnvironment env, IEmailProvider emailProvider)
        {
            _env = env;
            _notyf = notyf;
            _context = context;
            _emailProvider = emailProvider;
            menuBuilder = new MenuBuilder(context);
        }

        [Authorize(Roles = "System Setup")]
        public IActionResult SysSetup()
        {
            var userId = HttpContext.User.FindFirst(StrValues.UserId)?.Value ?? "";
            menuBuilder.BuildMenus(this, userId, "System Setup");
            var setting = _context.SysSetup.FirstOrDefault();
            return View(setting);
        }

        [Authorize(Roles = "System Setup")]
        [HttpPost]
        public IActionResult SysSetup([Bind("Name,Initials,Address,LogoImageUrl,ThemeColor,SecondaryColor,SmtpServer,SmtpUserName,SmtpPassword,SmtpPort,SocketOption")] SysSetup setting)
        {
            try
            {
                var userId = HttpContext.User.FindFirst(StrValues.UserId)?.Value ?? "";
                menuBuilder.BuildMenus(this, userId, "System Setup");

                if (string.IsNullOrEmpty(setting.Name))
                {
                    _notyf.Error("Kindly provide Org Name");
                    return View(setting);
                }
                
                var portalSetting = _context.SysSetup.FirstOrDefault();
                portalSetting.Name = setting.Name;
                portalSetting.Initials = setting.Initials;
                portalSetting.Address = setting.Address;
                portalSetting.LogoImageUrl = setting.LogoImageUrl;
                portalSetting.ThemeColor = setting.ThemeColor;
                portalSetting.SecondaryColor = setting.SecondaryColor;
                portalSetting.SmtpServer = setting.SmtpServer;
                portalSetting.SmtpUserName = setting.SmtpUserName;
                portalSetting.SmtpPassword = string.IsNullOrEmpty(setting.SmtpPassword) ? portalSetting.SmtpPassword : Decryptor.Encrypt(setting.SmtpPassword);
                portalSetting.SmtpPort = setting.SmtpPort;
                portalSetting.SocketOption = setting.SocketOption;
               
                _context.SaveChanges();
                _notyf.Success("System Setup updated successfully");
            }
            catch (Exception)
            {
                _notyf.Error("Sorry, An error occurred");
            }
            return View(setting);
        }
      
        [Authorize(Roles = "Users")]
        public async Task<IActionResult> ListUsers(FilterUsersVm? filter)
        {
            filter.Page = filter?.Page ?? 1;
            var userId = HttpContext.User.FindFirst(StrValues.UserId)?.Value ?? "";
            menuBuilder.BuildMenus(this, userId, "Users");

            filter.Page = filter.Page < 1 ? 1 : filter.Page;
            var users = await GetUsers(filter);
            int totalItems = users.Count();
            var pager = new Pager(totalItems, filter.Page, StrValues.PageSize);
            int skip = (filter.Page - 1) * StrValues.PageSize;
            var data = users.Skip(skip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;
            return View(new UserListFormVm
            {
                List = data,
                Filter = filter
            });
        }

        private async Task<List<User>> GetUsers(FilterUsersVm filter)
        {
            var users = await _context.Users.Where(u => (string.IsNullOrEmpty(filter.UserID) || u.UserID.ToUpper().Equals(filter.UserID.ToUpper()))
            && (string.IsNullOrEmpty(filter.Site) || u.Site.ToUpper().Equals(filter.Site.ToUpper())))
                .OrderByDescending(s => s.Id).ToListAsync();
            if (!filter.InActive)
                users = users.Where(u => u.Status).ToList();

            return users;
        }

        private async Task<List<UserGroup>> GetUserGroups(FilterUserGroupsVm filter)
        {
            var userGroups = await _context.UserGroups.Where(u => (string.IsNullOrEmpty(filter.UserGroup) || u.Name.ToUpper().Equals(filter.UserGroup.ToUpper())))
                .OrderByDescending(s => s.Id).ToListAsync();
            if (!filter.InActive)
                userGroups = userGroups.Where(u => !u.Closed).ToList();

            return userGroups;
        }

        [Authorize]
        public async Task<IActionResult> PdfUsers(FilterUsersVm? filter)
        {
            var userId = HttpContext.User.FindFirst(StrValues.UserId)?.Value ?? "";
            menuBuilder.BuildMenus(this, userId, "Users");
            var users = await GetUsers(filter);
            
            return new ViewAsPdf(users);
        }

        [Authorize]
        public async Task<IActionResult> ExcelUsers(FilterUsersVm? filter)
        {
            var userId = HttpContext.User.FindFirst(StrValues.UserId)?.Value ?? "";
            menuBuilder.BuildMenus(this, userId, "Users");
            var users = await GetUsers(filter);
            var currentRow = 1;
            var now = DateTime.UtcNow.AddHours(3);
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Users");
            worksheet.Cell(currentRow, 2).Value = ""+ now;
            currentRow++;
            worksheet.Cell(currentRow, 2).Value = "Users";
            currentRow++;
           
            worksheet.Rows(currentRow, currentRow).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Value = "User ID";
            worksheet.Cell(currentRow, 2).Value = "Names";
            worksheet.Cell(currentRow, 3).Value = "Email";
            worksheet.Cell(currentRow, 4).Value = "Phone";
            worksheet.Cell(currentRow, 5).Value = "Site";
            foreach (var user in users)
            {
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = user.UserID;
                worksheet.Cell(currentRow, 2).Value = user.Names;
                worksheet.Cell(currentRow, 3).Value = user.Email;
                worksheet.Cell(currentRow, 4).Value = user.Phone;
                worksheet.Cell(currentRow, 5).Value = user.Site;
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return File(content,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Users__{now}.xlsx");
            }
        }

        [Authorize]
        public async Task<IActionResult> ExcelUserGroups(FilterUserGroupsVm? filter)
        {
            var userId = HttpContext.User.FindFirst(StrValues.UserId)?.Value ?? "";
            menuBuilder.BuildMenus(this, userId, "User Group");
            var users = await GetUserGroups(filter);
            var currentRow = 1;
            var now = DateTime.UtcNow.AddHours(3);
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("User Groups");
            worksheet.Cell(currentRow, 2).Value = "" + now;
            currentRow++;
            worksheet.Cell(currentRow, 2).Value = "User Groups";
            currentRow++;

            worksheet.Rows(currentRow, currentRow).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Value = "UserGroup";
            worksheet.Cell(currentRow, 2).Value = "Closed";
            foreach (var user in users)
            {
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = user.Name;
                worksheet.Cell(currentRow, 2).Value = user.Closed;
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return File(content,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"User Groups__{now}.xlsx");
            }
        }

        [Authorize]
        public async Task<IActionResult> PdfUserGroups(FilterUserGroupsVm? filter)
        {
            var userId = HttpContext.User.FindFirst(StrValues.UserId)?.Value ?? "";
            menuBuilder.BuildMenus(this, userId, "User Group");
            var sites = await GetUserGroups(filter);

            return new ViewAsPdf(sites);
        }

        [Authorize(Roles = "Users")]
        public IActionResult NewUser()
        {
            InitializeUsers();
            return View(new User
            {
                AccessLevel = AccessLevel.Site,
                Status = true
            });
        }

        private void InitializeUsers()
        {
            var userId = HttpContext.User.FindFirst(StrValues.UserId)?.Value ?? "";
            menuBuilder.BuildMenus(this, userId, "Users");
           
            var groups = _context.UserGroups.Where(s => !s.Closed).Select(s => s.Name).ToList();
            ViewBag.groups = new SelectList(groups);
        }

        [Authorize(Roles = "Users")]
        [HttpPost]
        public JsonResult NewUser([FromBody] User user)
        {
            try
            {
                InitializeUsers();
                user.UserID = user?.UserID ?? "";
                if (string.IsNullOrEmpty(user.UserID))
                {
                    _notyf.Error("Kindly provide user Id");
                    return Json("");
                }
                if (string.IsNullOrEmpty(user.Password))
                {
                    _notyf.Error("Kindly provide password");
                    return Json("");
                }
                if (user.Password != user.ConfirmPassword)
                {
                    _notyf.Error("Sorry, Password and confirm password do not match");
                    return Json("");
                }
                if (_context.Users.Any(s => s.UserID.ToUpper().Equals(user.UserID.ToUpper())))
                {
                    _notyf.Error("Sorry, User already exist");
                    return Json("");
                }

                user.Password = Decryptor.Encrypt(user.Password);
                _context.Users.Add(user);
                _context.SaveChanges();
                _notyf.Success("User added successfully");
                return Json("");
            }
            catch (Exception)
            {
                _notyf.Error("Sorry, An error occurred");
                return Json("");
            }
        }

        [Authorize(Roles = "Users")]
        public IActionResult EditUser(int? id)
        {
            InitializeUsers();
            var user = _context.Users.Include(u => u.AssignedGroups).FirstOrDefault(u => u.Id == id);
            if(user == null)
            {
                _notyf.Error("Sorry, User not found");
                return View(user);
            }
            
            return View(user);
        }

        [Authorize(Roles = "Users")]
        [HttpPost]
        public JsonResult EditUser([FromBody] User user)
        {
            try
            {
                InitializeUsers();
                user.UserID = user?.UserID ?? "";
                if (string.IsNullOrEmpty(user.UserID))
                {
                    _notyf.Error("Kindly provide user Id");
                    return Json("");
                }
                if (user.Password != user.ConfirmPassword)
                {
                    _notyf.Error("Sorry, Password and confirm password do not match");
                    return Json("");
                }
                if (_context.Users.Any(s => s.UserID.ToUpper().Equals(user.UserID.ToUpper()) && s.Id != user.Id))
                {

                    _notyf.Error("Sorry, User already exist");
                    return Json("");
                }

                var savedUser = _context.Users.FirstOrDefault(u => u.Id ==  user.Id);
                user.Password = string.IsNullOrEmpty(user.Password) ? savedUser.Password : Decryptor.Encrypt(user.Password);
                if (savedUser != null)
                {
                    var assignedGroups = _context.AssignedGroups.Where(p => p.UserId == savedUser.Id);
                    if(assignedGroups.Any())
                        _context.AssignedGroups.RemoveRange(assignedGroups);
                    _context.Users.Remove(savedUser);

                    _context.Users.Add(user);
                    _context.SaveChanges();
                }


                _notyf.Success("User update successfully");

                return Json("");
            }
            catch (Exception)
            {
                _notyf.Error("Sorry, An error occurred");
                return Json("");
            }
        }

        [Authorize(Roles = "User Group")]
        public async Task<IActionResult> ListUserGroups(FilterUserGroupsVm? filter)
        {
            filter.Page = filter?.Page ?? 1;
            var userId = HttpContext.User.FindFirst(StrValues.UserId)?.Value ?? "";
            menuBuilder.BuildMenus(this, userId, "User Group");

            filter.Page = filter.Page < 1 ? 1 : filter.Page;
            var userGroups = await GetUserGroups(filter);
            int totalItems = userGroups.Count();
            var pager = new Pager(totalItems, filter.Page, StrValues.PageSize);
            int skip = (filter.Page - 1) * StrValues.PageSize;
            var data = userGroups.Skip(skip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;
            return View(new UsergroupListFormVm
            {
                List = data,
                Filter = filter
            });
        }

        [Authorize(Roles = "User Group")]
        public IActionResult NewUserGroup()
        {
            var userId = HttpContext.User.FindFirst(StrValues.UserId)?.Value ?? "";
            menuBuilder.BuildMenus(this, userId, "User Group");

            var privileges = PrivilegeBuilder.GetPrivileges(new List<UserPrivilege>(), GroupOperation.GroupEditing);
            return View(new UsergroupVm
            {
                Menus = privileges,
            });
        }

        [Authorize(Roles = "User Group")]
        [HttpPost]
        public JsonResult NewUserGroup([FromBody] UserGroup userGroup)
        {
            try
            {
                var userId = HttpContext.User.FindFirst(StrValues.UserId)?.Value ?? "";
                menuBuilder.BuildMenus(this, userId, "User Group");

                if (string.IsNullOrEmpty(userGroup.Name))
                {
                    _notyf.Error("Kindly provide the group");
                    return Json("");
                }

                if (_context.UserGroups.Any(g => g.Name.ToUpper().Equals(userGroup.Name.ToUpper())))
                {
                    _notyf.Error("Sorry, Usergroup already exist");
                    return Json("");
                }

                foreach (var privilege in userGroup.UserPrivileges)
                    privilege.PrivilegeCode = Decryptor.Encrypt(privilege.PrivilegeCode);

                _context.UserGroups.Add(userGroup);
                _context.SaveChanges();
                _notyf.Success("Usergroup added successfully");
                return Json("");
            }
            catch (Exception)
            {
                _notyf.Error("Sorry, An error occurred");
                return Json("");
            }
        }

        [Authorize(Roles = "User Group")]
        public IActionResult EditUserGroup(int? id)
        {
            var userId = HttpContext.User.FindFirst(StrValues.UserId)?.Value ?? "";
            menuBuilder.BuildMenus(this, userId, "User Group");

            var userGroup = _context.UserGroups.Include(g => g.UserPrivileges).FirstOrDefault(g => g.Id == id);
            if (userGroup == null)
            {
                _notyf.Error("Sorry, Group could not be found");
                return RedirectToAction("ListUserGroups");
            }

            var privileges = PrivilegeBuilder.GetPrivileges(userGroup.UserPrivileges, GroupOperation.GroupEditing);
            return View(new UsergroupVm
            {
                Id = userGroup.Id,
                Name = userGroup.Name,
                Closed = userGroup.Closed,
                Menus = privileges,
            });
        }

        [Authorize(Roles = "User Group")]
        [HttpPost]
        public JsonResult EditUserGroup([FromBody] UserGroup userGroup)
        {
            try
            {
                var userId = HttpContext.User.FindFirst(StrValues.UserId)?.Value ?? "";
                menuBuilder.BuildMenus(this, userId, "User Group");

                if (string.IsNullOrEmpty(userGroup.Name))
                {
                    _notyf.Error("Kindly provide the group");
                    return Json("");
                }

                if (_context.UserGroups.Any(g => g.Name.ToUpper().Equals(userGroup.Name.ToUpper()) && g.Id != userGroup.Id))
                {
                    _notyf.Error("Sorry, Usergroup already exist");
                    return Json("");
                }

                var group = _context.UserGroups.Include(g => g.UserPrivileges).FirstOrDefault(g => g.Id == userGroup.Id);
                if (group != null)
                {
                    if (group.UserPrivileges.Any())
                        _context.UserPrivileges.RemoveRange(group.UserPrivileges);
                    _context.UserGroups.Remove(group);
                }
                
                foreach (var privilege in userGroup.UserPrivileges)
                    privilege.PrivilegeCode = Decryptor.Encrypt(privilege.PrivilegeCode);

                _context.UserGroups.Add(userGroup);
                _context.SaveChanges();
                _notyf.Success("Usergroup updated successfully");
                return Json("");
            }
            catch (Exception)
            {
                _notyf.Error("Sorry, An error occurred");
                return Json("");
            }
        }
    }
}
