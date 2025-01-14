using System.ComponentModel.DataAnnotations;

namespace AAAErp.Models
{
    public class UserPrivilege
    {
        public int Id { get; set; }
        public int? UserGroupId { get; set; }
        [StringLength(100)]
        public string? PrivilegeCode { get; set; }
        public AccessRight? AccessRight { get; set; }
    }

    public enum AccessRight
    {
        Hidden = 1,
        Read = 2,
        Write = 3,
    }
}