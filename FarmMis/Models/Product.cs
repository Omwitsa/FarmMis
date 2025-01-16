using System.ComponentModel.DataAnnotations;

namespace FarmMis.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int? VegId { get; set; }
        [StringLength(50)]
        public string? Code { get; set; }
        [StringLength(250)]
        public string? Name { get; set; }
        [StringLength(250)]
        public string? Type { get; set; }
        [StringLength(250)]
        public string? Category { get; set; }
    }
}