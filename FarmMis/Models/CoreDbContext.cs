using Microsoft.EntityFrameworkCore;

namespace FarmMis.Models
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
        public virtual DbSet<UserGroup> UserGroups { get; set; }
        public virtual DbSet<UserPrivilege> UserPrivileges { get; set; }
        public virtual DbSet<AssignedGroup> AssignedGroups { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Branch> Branches { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Packlist> Packlists { get; set; }
        public virtual DbSet<PacklistLine> PacklistLines { get; set; }
    }
}
