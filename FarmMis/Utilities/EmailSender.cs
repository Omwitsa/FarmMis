using AAAErp.Constants;
using AAAErp.IProvider;
using AAAErp.Models;
using AAAErp.Models.MISModel;
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

        public async Task<ReturnData<bool>> SendIngressbackup(SysSetup setting)
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

        public async Task<ReturnData<bool>> SendBirthDayEmails(SysSetup setting, HrSetup hrSetup, List<HrEmployee> employees, List<Farm> farms)
        {
            try
            {
                //var varificationLink = $"{Request.Host}/account/confirmEmail?username={userDetails.Data.UserId}";
                var institutionEmail = new EmailAddress
                {
                    Name = "Happy Birthday",//setting.Name,
                    Address = setting.HrMail
                };

                var smtpSettings = new MailSettings
                {
                    EmailId = setting.HrMail,
                    Password = Decryptor.Decrypt(setting.HrMailPwd),
                    Server = setting.SmtpServer,
                    Port = setting.SmtpPort,
                    SocketOption = setting.SocketOption
                };

                var subject = hrSetup.BirthdayMsgSbj.Replace(Values.CompanyNameHolder, setting.Name);
                var birthdayImg = Path.Combine(_env.WebRootPath, hrSetup.BirthdayImg);

                var reportMessage = "";
                foreach (var employee in employees)
                {
                    var farm = farms.FirstOrDefault(f => f.FarmId == employee.FarmId);
                    reportMessage = $"{reportMessage} <tr><td> {employee.EmployeeName} ({farm.FarmName}) <br><br></td></tr>";

                    var receiverEmail = new EmailAddress
                    {
                        Name = employee.EmployeeName,
                        Address = employee.Email
                    };
                    
                    var emailMessage = new EmailMessage
                    {
                        FromAddresses = new List<EmailAddress> { institutionEmail },
                        ToAddresses = new List<EmailAddress> { receiverEmail },
                        Subject = subject,
                        BirthdayImg = birthdayImg,
                        //Attachments = new List<string>(),
                        Body = MailGenerator.GenerateBirthDayMessage(receiverEmail, hrSetup),
                    };
                    //emailMessage.Attachments.Add(birthdayImg);

                    await _emailProvider.SendEmailAsync(emailMessage, smtpSettings, MailOparation.Birthdaymail);
                }

                if (!employees.Any())
                    reportMessage = "<tr><td> No staff has birthday today <br><br></td></tr>";

                var receiver1 = new EmailAddress
                {
                    Name = "Happy Birthday",//setting.Name,
                    Address = setting.HrMail
                };

                var receiver2 = new EmailAddress
                {
                    Name = "Happy Birthday",//setting.Name,
                    Address = "it@aaagrowers.co.ke"
                };

                var birthdayReport = new EmailMessage
                {
                    FromAddresses = new List<EmailAddress> { institutionEmail },
                    ToAddresses = new List<EmailAddress> { receiver1, receiver2 },
                    Subject = "Birthday Notification",
                    //Attachments = new List<string>(),
                    Body = MailGenerator.GenerateBirthdayReport(reportMessage),
                };

                await _emailProvider.SendEmailAsync(birthdayReport, smtpSettings, MailOparation.Default);

                return new ReturnData<bool>
                {
                    Success = true,
                    Message = ""
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"CustomError: EmailSender SendBirthDayEmails: {ex.Message}");
                return new ReturnData<bool>
                {
                    Success = false,
                    Message = "Sorry, An error occurred"
                };
            }
        }

    }
}
