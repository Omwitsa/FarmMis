using System.ComponentModel.DataAnnotations;

namespace FarmMis.Models
{
    public class Branch
    {
        public int Id { get; set; }
        public int? VegId { get; set; }
        [StringLength(250)]
        public string? Name { get; set; }
        public int? CustomerId { get; set; }
    }
}