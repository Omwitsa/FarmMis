using System.ComponentModel.DataAnnotations;

namespace AAAErp.Models.MISModel
{
    public class HrEmployee
    {
        [Key]
        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public string? Telephone { get; set; }
        public string? Email { get; set; }
        public string? PostalAddress { get; set; }
        public string? NationalId { get; set; }
        public string? PayrollNumber { get; set; }
        public int? FarmId { get; set; }
    }
}
