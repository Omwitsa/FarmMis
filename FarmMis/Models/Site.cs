using System.ComponentModel.DataAnnotations;

namespace AAAErp.Models
{
    public class Site
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string? Name { get; set; }
        [StringLength(200)]
        public string? Contact { get; set; }
        [StringLength(50)]
        public string? IngressUserName { get; set; }
        [StringLength(100)]
        public string? IngressPassword { get; set; }
        [StringLength(50)]
        public string? IngressServer { get; set; }
        [StringLength(20)]
        public string? IngressDb { get; set; }
        [StringLength(20)]
        public string? HoIngressDb { get; set; }
        [StringLength(100)]
        public string? LastBackup { get; set; }
        public bool Closed { get; set; }
        [StringLength(50)]
        public string? Personnel { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}