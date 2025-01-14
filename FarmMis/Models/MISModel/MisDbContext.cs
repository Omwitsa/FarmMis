using Microsoft.EntityFrameworkCore;

namespace AAAErp.Models.MISModel
{
    public partial class MisDbContext : DbContext
    {
        public MisDbContext()
        {
        }

        public MisDbContext(DbContextOptions<MisDbContext> options)
        : base(options)
        {
        }

        public virtual DbSet<HrEmployee> hr_employee { get; set; }
        public virtual DbSet<Farm> hr_farm { get; set; }
    }
}
