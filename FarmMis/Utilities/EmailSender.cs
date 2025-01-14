using AAAErp.Constants;
using AAAErp.IProvider;
using AAAErp.Models;
using AAAErp.Provider;
using AAAErp.ViewModel;
using System.Net.Mail;
using static AAAErp.Constants.Values;

namespace AAAErp.Utilities
{
    public class EmailSender
    {
        private readonly IWebHostEnvironment _env;
        private readonly IEmailProvider _emailProvider;
        private readonly ILogger<CronJobProvider> _logger;
        public EmailSender(IWebHostEnvironment env, IEmailProvider emailProvider, ILogger<CronJobProvider> logger) {
            _env = env;
            _logger = logger;
            _emailProvider = emailProvider;
        }

        public async Task<ReturnData<bool>> SendMail(SysSetup setting)
        {
            //var varificationLink = $"{Request.Host}/account/confirmEmail?username={userDetails.Data.UserId}";
            //var logoImageUrl = Path.Combine(_env.WebRootPath, setting.LogoImageUrl);
            var institutionEmail = new EmailAddress
            {
                Name = setting.Name,
                Address = setting.SmtpUserName
            };

            var receiverEmail = new EmailAddress
            {
                Name = "Ingress Backup",
                Address = "wilson.omwitsa@aaagrowers.co.ke"
            };

            var emailMessage = new EmailMessage
            {
                FromAddresses = new List<EmailAddress> { institutionEmail },
                ToAddresses = new List<EmailAddress> { receiverEmail },
                Subject = $"{setting.Name} Ingress Backup",
                //InstitutionLogo = logoImageUrl,
                //Attachments = new List<string>(),
                Body = MailGenerator.GenerateMailBody(institutionEmail),
            };

            var filePath = Path.Combine(_env.WebRootPath, "MySQL", "backup.sql.zip");
            //emailMessage.Attachments.Add(filePath);
            var smtpSettings = new MailSettings
            {
                EmailId = setting.SmtpUserName,
                Password = setting.SmtpPassword,
                Server = setting.SmtpServer,
                Port = setting.SmtpPort,
                SocketOption = setting.SocketOption
            };
            return await _emailProvider.SendEmailAsync(emailMessage, smtpSettings, MailOparation.Default);
        }

    }
}
