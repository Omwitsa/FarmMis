using System.ComponentModel.DataAnnotations;

namespace FarmMis.Models
{
    public class UserGroup
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string? Name { get; set; }
        public IEnumerable<UserPrivilege>? UserPrivileges { get; set; }
        public bool Closed { get; set; }
        [StringLength(50)]
        public string? Personnel { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}