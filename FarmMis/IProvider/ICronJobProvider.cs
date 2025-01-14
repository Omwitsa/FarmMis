using AAAErp.Models.MISModel;

namespace AAAErp.IProvider
{
    public interface ICronJobProvider
    {
        Task SendBirtdayMails();
    }
}
