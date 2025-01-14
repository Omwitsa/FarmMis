using System.ComponentModel.DataAnnotations;

namespace AAAErp.ViewModel
{
    public class IngressVm
    {
        [StringLength(50)]
        public string Site { get; set; }
    }

    public class DbConnectionVm
    {
        public string? Query { get; set; }
        public string? Server { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Db { get; set; }
    }
}
