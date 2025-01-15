using FarmMis.Controllers;
using FarmMis.IProvider;
using FarmMis.ViewModel;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using static FarmMis.Constants.Values;

namespace FarmMis.Provider
{
    public class EmailProvider : IEmailProvider
    {
        private readonly ILogger<HomeController> _logger;
        public EmailProvider(ILogger<HomeController> logger) {
            _logger = logger;
        }
        public List<EmailMessage> ReceiveEmail(int maxCount = 10)
        {
            throw new NotImplementedException();
        }

        public async Task<ReturnData<bool>> SendEmailAsync(EmailMessage emailMessage, MailSettings smtpSettings, MailOparation mailOparation)
        {
            var email = new MimeMessage();
            email.To.AddRange(emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
            email.From.AddRange(emailMessage.FromAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
            email.Subject = emailMessage.Subject;

            var builder = new BodyBuilder();
            //foreach (var file in emailMessage.Attachments)
            //    builder.Attachments.Add(file);

            if(MailOparation.Birthdaymail == mailOparation)
            {
                var image = builder.LinkedResources.Add(emailMessage.BirthdayImg);
                image.ContentId = "birthdayImgId";
            }

            builder.HtmlBody = emailMessage.Body;
            email.Body = builder.ToMessageBody();

            //var options = SecureSocketOptions.StartTls;
            //if (_mailSettings.SocketOption.ToLower().Equals("sslonconnect"))
            //    options = SecureSocketOptions.SslOnConnect;
            //if (_mailSettings.SocketOption.ToLower().Equals("none"))
            //    options = SecureSocketOptions.None;

            try
            {
                using var smtp = new SmtpClient();
                smtp.Connect(smtpSettings.Server, smtpSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(smtpSettings.EmailId, smtpSettings.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
                return new ReturnData<bool>
                {
                    Success = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"CustomError: EmailProvider SendEmailAsync {ex.Message}");
                return new ReturnData<bool>
                {
                    Success = false,
                    Message = "Sorry, An error occurred while sending email"
                };
            }
        }
    }
}
