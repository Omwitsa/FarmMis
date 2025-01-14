using System.ComponentModel.DataAnnotations;

namespace AAAErp.Models
{
    public class HrSetup
    {
        public int Id { get; set; }
        public string? BirthdayMsg { get; set; }
        public string? BirthdayMsgSbj { get; set; }
        public DateTime? LastBirthdayWishDate { get; set; }
        public int? NoOfStaffWished { get; set; }
        public string? BirthdayImg { get; set; }
    }
}