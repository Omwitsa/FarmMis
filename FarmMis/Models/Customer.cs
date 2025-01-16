using System.ComponentModel.DataAnnotations;

namespace FarmMis.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public int? VegId { get; set; }
        [StringLength(50)]
        public string? Code { get; set; }
        [StringLength(250)]
        public string? Name { get; set; }
    }
}