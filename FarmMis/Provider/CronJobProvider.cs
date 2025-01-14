using AAAErp.Controllers;
using AAAErp.IProvider;
using AAAErp.Models;
using AAAErp.Models.MISModel;
using AAAErp.Utilities;
using AAAErp.ViewModel;

namespace AAAErp.Provider
{
    public class CronJobProvider : ICronJobProvider
    {
        private CoreDbContext _context;
        private MisDbContext _misDbContext;
        private EmailSender emailSender;
        private readonly ILogger<CronJobProvider> _logger;
        public CronJobProvider(CoreDbContext context, MisDbContext misDbContext, IEmailProvider emailProvider, IWebHostEnvironment env,
            ILogger<CronJobProvider> logger)
        {
            _logger = logger;
            _context = context;
            _misDbContext = misDbContext;
            emailSender = new EmailSender(env, emailProvider, logger);
        }

        public async Task SendBirtdayMails()
        {
            //var birthDay = DateTime.Parse("1902-11-23");
            var employees = _misDbContext.hr_employee.Where(e => e.BirthDate != null && e.Email != null && e.TerminationDate == null).ToList();
            employees = employees.Where(e => e.BirthDate.GetValueOrDefault().Month == DateTime.Today.Month && e.BirthDate.GetValueOrDefault().Day == DateTime.Today.Day).ToList();

            var sysSetup = _context.SysSetup.FirstOrDefault();
            var hrSetup = _context.HrSetup.FirstOrDefault();
            var farms = _misDbContext.hr_farm.ToList();

            if (hrSetup.LastBirthdayWishDate != DateTime.Today)
            {
                await emailSender.SendBirthDayEmails(sysSetup, hrSetup, employees, farms);
                hrSetup.LastBirthdayWishDate = DateTime.Today;
                hrSetup.NoOfStaffWished = employees.Count;
                _context.SaveChanges();
            }
        }
    }
}
