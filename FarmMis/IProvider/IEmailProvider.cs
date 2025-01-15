using FarmMis.ViewModel;
using static FarmMis.Constants.Values;

namespace FarmMis.IProvider
{
    public interface IEmailProvider
    {
        Task<ReturnData<bool>> SendEmailAsync(EmailMessage message, MailSettings smtpSettings, MailOparation mailOparation);
        List<EmailMessage> ReceiveEmail(int maxCount = 10);
    }
}
