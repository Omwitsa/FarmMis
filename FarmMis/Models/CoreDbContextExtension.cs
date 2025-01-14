using AAAErp.Constants;
using AAAErp.Utilities;

namespace AAAErp.Models
{
    public static class CoreDbContextExtension
    {
        public static void EnsureDatabaseSeeded(this CoreDbContext context)
        {
            if (!context.SysSetup.Any())
            {
                context.Add(new SysSetup
                {
                    Name = "AAA",
                    SmtpServer = "mail.aaagrowers.co.ke",
                    SmtpUserName = "ingress.bkpauto@aaagrowers.co.ke",
                    SmtpPassword = Decryptor.Encrypt("Ib@$#@9les!"),
                    SmtpPort = 587, // 587, 465
                    SocketOption = "TLS", // TLS, SSL, NONE
                });
            }

            if(!context.UserGroups.Any())
            {
                var userPrivileges = new List<UserPrivilege>();
                foreach (var privilege in ArrValues.Privileges)
                    userPrivileges.Add(new UserPrivilege {
                        PrivilegeCode = Decryptor.Encrypt(privilege.Code),
                        AccessRight = AccessRight.Write
                    });

                context.Add(new UserGroup
                {
                    UserPrivileges = userPrivileges,
                    Name = "Super Admin",
                    Closed = false,
                });
            }

            if (!context.Users.Any())
            {
                var assignedGroups = new List<AssignedGroup>();
                assignedGroups.Add(new AssignedGroup { Group = "Super Admin" });
                context.Add(new User
                {
                    UserID = "Admin@aaa.com",
                    Names = "Administrator",
                    DateCreated = DateTime.UtcNow,
                    Password = Decryptor.Encrypt("Admin.123"),
                    Site = "HO",
                    Status = true,
                    AccessLevel = AccessLevel.General,
                    AssignedGroups = assignedGroups,
                });
            }

            context.SaveChanges();
        }
    }
}
