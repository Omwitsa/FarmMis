using AAAErp.Models;
using AAAErp.ViewModel;
using NuGet.Protocol.Plugins;

namespace AAAErp.Utilities
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

        public static string GenerateBirthDayMessage(EmailAddress sender, HrSetup hrSetup)
        {
            hrSetup.BirthdayMsg = hrSetup?.BirthdayMsg ?? "";
            if (!string.IsNullOrEmpty(hrSetup.BirthdayMsg))
            {
                var messageArr = hrSetup.BirthdayMsg.Split("\r\n");
                for (int i = 0; i < messageArr.Length; i++)
                    messageArr[i] = $"<tr><td> {messageArr[i]} <br> <br></td></tr>";

                hrSetup.BirthdayMsg = string.Join("", messageArr);
            }

            return "<div style='margin: 2em 2em 2em 2em; background-color: #f2f2f2'>" +
                           "<table style='width: 100%; margin: 5% 5% 5% 5%; text-align: center;'><br>" +
                                "<tr><td><h4>Dear " + sender.Name + ",</h4></td></tr>" + hrSetup.BirthdayMsg +
                                "<tr><td><img src='cid:birthdayImgId' style='width:70%; padding: inherit; display: block; margin:auto;'/> <br><br></td></tr>" +
                                "<b>AAA GROWERS LIMITED.</b></a> &copy; Copyright <span id='c-year'>" + DateTime.UtcNow.Year + "</span></p><br><br>" +
                           " </table>" +
                       "</div>";
        }

        public static string GenerateBirthdayReport(string reportMessage)
        {
            return "<div style='margin: 2em 5em 2em 5em; background-color: #f2f2f2'>" +
                       "<table style='width: 100 %; margin: 5% 10% 5% 10%;'><br>" +
                            "<tr><td><h4>Hello Admin,</h4></td></tr>" +
                            "<tr><td> Please note the following staff member(s) have birthday(s) today. <br><br></td></tr>" + reportMessage +
                            "<p>This is a system generated report</p><br><br>" +
                       " </table>" +
                   "</div>";
        }
    }
}
