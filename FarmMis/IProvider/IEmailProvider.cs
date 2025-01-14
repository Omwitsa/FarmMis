using AAAErp.ViewModel;
using static AAAErp.Constants.Values;

namespace AAAErp.IProvider
{
    public interface IEmailProvider
    {
        Task<ReturnData<bool>> SendEmailAsync(EmailMessage message, MailSettings smtpSettings, MailOparation mailOparation);
        List<EmailMessage> ReceiveEmail(int maxCount = 10);
    }
}
