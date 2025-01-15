using FarmMis.Controllers;
using FarmMis.IProvider;
using FarmMis.Models;
using FarmMis.Utilities;
using FarmMis.ViewModel;

namespace FarmMis.Provider
{
    public class CronJobProvider : ICronJobProvider
    {
        private CoreDbContext _context;
        private EmailSender emailSender;
        private readonly ILogger<CronJobProvider> _logger;
        public CronJobProvider(CoreDbContext context, IEmailProvider emailProvider, IWebHostEnvironment env,
            ILogger<CronJobProvider> logger)
        {
            _logger = logger;
            _context = context;
            emailSender = new EmailSender(env, emailProvider, logger);
        }

        public async Task SendMail()
        {
            var sysSetup = _context.SysSetup.FirstOrDefault();
            await emailSender.SendMail(sysSetup);
        }
    }
}
