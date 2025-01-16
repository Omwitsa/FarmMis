using System.ComponentModel.DataAnnotations;

namespace FarmMis.Models
{
    public class Packlist
    {
        public int Id { get; set; }
        public int? VegHeaderId { get; set; }
        public DateTime? DispatchDate { get; set; }
        public int? CustomerId { get; set; }
        [StringLength(250)]
        public string? CustomerName { get; set; }
        public int? BranchId { get; set; }
        [StringLength(250)]
        public string? BranchName { get; set; }
        public IEnumerable<PacklistLine> PacklistLines { get; set; }
    }
}