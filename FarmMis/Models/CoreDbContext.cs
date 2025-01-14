using Microsoft.EntityFrameworkCore;

namespace AAAErp.Models
{
    public partial class CoreDbContext : DbContext
    {
        public CoreDbContext()
        {
        }

        public CoreDbContext(DbContextOptions<CoreDbContext> options)
        : base(options)
        {
        }

        public virtual DbSet<SysSetup> SysSetup { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Site> Sites { get; set; }
        public virtual DbSet<UserGroup> UserGroups { get; set; }
        public virtual DbSet<UserPrivilege> UserPrivileges { get; set; }
        public virtual DbSet<AssignedGroup> AssignedGroups { get; set; }
        public virtual DbSet<HrSetup> HrSetup { get; set; }
    }
}
