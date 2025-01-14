using AAAErp.Constants;
using AAAErp.Models;
using AAAErp.Utilities;
using AAAErp.ViewModel;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mysqlx;
using System.Net.Http.Headers;

namespace AAAErp.Controllers
{
    public class HrController : Controller
    {
        private readonly INotyfService _notyf;
        private readonly CoreDbContext _context;
        private readonly IWebHostEnvironment _env;
        private MenuBuilder menuBuilder;
        public HrController(CoreDbContext context, INotyfService notyf, IWebHostEnvironment env)
        {
            _context = context;
            _notyf = notyf;
            _env = env;
            menuBuilder = new MenuBuilder(context);
        }

        [Authorize(Roles = "HR Setup")]
        public IActionResult HrSetup()
        {
            var userId = HttpContext.User.FindFirst(StrValues.UserId)?.Value ?? "";
            menuBuilder.BuildMenus(this, userId, "HR Setup");
            var setting = _context.HrSetup.FirstOrDefault();

            /**
              public string? BirthdayMsg { get; set; }
        public string? BirthdayMsgSbj { get; set; }
        public IFormFile? BirthdayImg { set; get; }
             */
            return View(new SettingVm
            {
                BirthdayMsgSbj = setting.BirthdayMsgSbj,
                BirthdayMsg = setting.BirthdayMsg,
            });
        }

        [Authorize(Roles = "HR Setup")]
        [HttpPost]
        public IActionResult HrSetup([Bind("BirthdayMsgSbj,BirthdayMsg")] SettingVm setting)
        {
            try
            {
                var userId = HttpContext.User.FindFirst(StrValues.UserId)?.Value ?? "";
                menuBuilder.BuildMenus(this, userId, "HR Setup");

                if (string.IsNullOrEmpty(setting.BirthdayMsgSbj))
                {
                    _notyf.Error("Kindly provide mail subject");
                    return View(setting);
                }

                var file = Request.Form.Files.FirstOrDefault();
                if (file == null)
                {
                    _notyf.Error("Kindly attach birthday image");
                    return View(setting);
                }

                string folderName = @"files\assets\images";
                string imageFolder = Path.Combine(_env.WebRootPath, folderName);
                if (!Directory.Exists(imageFolder))
                    Directory.CreateDirectory(imageFolder);

                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
               
                var imagePath = Path.Combine(imageFolder, fileName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                var portalSetting = _context.HrSetup.FirstOrDefault();
                portalSetting.BirthdayMsgSbj = setting.BirthdayMsgSbj;
                portalSetting.BirthdayMsg = setting.BirthdayMsg;
                portalSetting.BirthdayImg = Path.Combine(folderName, fileName);

                _context.SaveChanges();
                _notyf.Success("Hr Setup updated successfully");
            }
            catch (Exception)
            {
                _notyf.Error("Sorry, An error occurred");
            }
            return View(setting);
        }

    }
}
