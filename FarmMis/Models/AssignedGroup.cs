using System.ComponentModel.DataAnnotations;

namespace AAAErp.Models
{
    public class AssignedGroup
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        [StringLength(50)]
        public string? Group { get; set; }
    }
}