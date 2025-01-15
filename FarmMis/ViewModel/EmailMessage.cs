namespace FarmMis.ViewModel
{
    public class EmailMessage
    {
        public List<EmailAddress> ToAddresses { get; set; }
        public List<EmailAddress> FromAddresses { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string BirthdayImg { get; set; }
        public List<string> Attachments { get; set; }
    }

    public class MailSettings
    {
        public string EmailId { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public string SocketOption { get; set; }
    }

    public class EmailAddress
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
