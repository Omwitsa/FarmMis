using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmMis.Models
{
    public class User
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string? UserID { get; set; }
        [StringLength(50)]
        public string? Names { get; set; }
        [StringLength(100)]
        public string? Password { get; set; }
        [NotMapped]
        [Display(Name = "Confirm Password")]
        [StringLength(100)]
        public string? ConfirmPassword { get; set; }
        [StringLength(50)]
        public string? Email { get; set; }
        [StringLength(20)]
        public string? Phone { get; set;}
        [StringLength(50)]
        public string? Site { get; set;}
        public bool Status { get; set; }
        public AccessLevel? AccessLevel { get; set; }
        public IEnumerable<AssignedGroup>? AssignedGroups { get; set; }
        [StringLength(50)]
        public string? Personnel { get; set; } 
        public DateTime? DateCreated { get; set; }
    }

    public enum AccessLevel
    {
        General = 1,
        Site = 2,
    }
}
