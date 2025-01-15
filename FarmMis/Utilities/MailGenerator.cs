using FarmMis.Models;
using FarmMis.ViewModel;
using NuGet.Protocol.Plugins;

namespace FarmMis.Utilities
{
    public class MailGenerator
    {
        public static string GenerateMailBody(EmailAddress sender)
        {
            return "<div style='margin: 2em 5em 2em 5em; background-color: #f2f2f2'>" +
                       "<table style='width: 100 %; margin: 5% 10% 5% 10%;'><br>" +
                            "<tr><td> This is a system generated notification on " + sender.Name + " attendance data update as at (" + DateTime.UtcNow.AddHours(3) + ") <br> <br></td></tr>" +
                       " </table>" +
                   "</div>";
        }
    }
}
