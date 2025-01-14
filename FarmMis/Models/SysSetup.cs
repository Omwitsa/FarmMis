using System.ComponentModel.DataAnnotations;

namespace AAAErp.Models
{
    public class SysSetup
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string? Name { get; set; }
        [StringLength(20)]
        public string? Initials { get; set; }
        [StringLength(100)]
        public string? Address { get; set; }
        [StringLength(200)]
        public string? LogoImageUrl { get; set; }
        [StringLength(50)]
        public string? ThemeColor { get; set; }
        [StringLength(50)]
        public string? SecondaryColor { get; set; }
        [StringLength(50)]
        public string? SmtpServer { get; set; }
        [StringLength(50)]
        public string? SmtpUserName { get; set; }
        [StringLength(100)]
        public string? SmtpPassword { get; set; }
        public int SmtpPort { get; set; }
        [StringLength(20)]
        public string? SocketOption { get; set; }
        [StringLength(50)]
        public string? HrMail { get; set; }
        [StringLength(100)]
        public string? HrMailPwd { get; set; }
        [StringLength(50)]
        public string? IngressUserName { get; set; }
        [StringLength(100)]
        public string? IngressPassword { get; set; }
        [StringLength(50)]
        public string? IngressServer { get; set; }
        public int IngressBackMonths { get; set; }
        [StringLength(250)]
        public string? BackupLoc { get; set; }
    }
}