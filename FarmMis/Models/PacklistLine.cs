using System.ComponentModel.DataAnnotations;

namespace FarmMis.Models
{
    public class PacklistLine
    {
        public int Id { get; set; }
        public int? VegLineId { get; set; }
        public int? PacklistId { get; set; }
        public int? VegHeaderId { get; set; }
        public int? ProductId { get; set; }
        public int? BoxQty { get; set; }
        [StringLength(50)]
        public string? Barcode { get; set; }
    }
}