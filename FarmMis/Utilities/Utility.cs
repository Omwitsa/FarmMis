using AAAErp.Constants;
using AAAErp.Models;
using AAAErp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AAAErp.Utilities
{
    public class DatabaseUpdator
    {
        public static void UpdateDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<CoreDbContext>();
                context.Database.Migrate();
                context.EnsureDatabaseSeeded();
                // context.Database.EnsureCreated();
            }
        }
    }

    public class PrivilegeBuilder
    {
        public static List<MainMenuVm> GetPrivileges(IEnumerable<UserPrivilege>? userPrivileges, GroupOperation groupOperation)
        {
            var level1Menus = ArrValues.Privileges.Where(p => string.IsNullOrEmpty(p.ParentCode)).ToList();
            var codes = userPrivileges.Select(p => p.PrivilegeCode).ToList();

            if (groupOperation == GroupOperation.MenuDisplay)
                level1Menus = level1Menus.Where(p => codes.Contains(Decryptor.Encrypt(p.Code))).ToList();
            var menuPrivileges = new List<MainMenuVm>();
            level1Menus.ForEach(l1 =>
            {
                l1.HasChild = ArrValues.Privileges.Any(p => p.ParentCode == l1.Code);
                l1.AccessRight = userPrivileges.FirstOrDefault(p => p.PrivilegeCode == Decryptor.Encrypt(l1.Code))?.AccessRight;
                var level2Privileges = ArrValues.Privileges.Where(p => p.ParentCode.Equals(l1.Code)).ToList();
                if (groupOperation == GroupOperation.MenuDisplay)
                    level2Privileges = level2Privileges.Where(p => codes.Contains(Decryptor.Encrypt(p.Code))).ToList();
                var level2Menus = new List<MenuLevel2Vm>();
                level2Privileges.ForEach(l2 =>
                {
                    l2.HasChild = ArrValues.Privileges.Any(p => p.ParentCode == l2.Code);
                    l2.AccessRight = userPrivileges.FirstOrDefault(p => p.PrivilegeCode == Decryptor.Encrypt(l2.Code))?.AccessRight;
                    var level3Privileges = ArrValues.Privileges.Where(p => p.ParentCode.Equals(l2.Code)).ToList();
                    if (groupOperation == GroupOperation.MenuDisplay)
                        level3Privileges = level3Privileges.Where(p => codes.Contains(Decryptor.Encrypt(p.Code))).ToList();
                    level3Privileges.ForEach(l3 => l3.AccessRight = userPrivileges.FirstOrDefault(p => p.PrivilegeCode == Decryptor.Encrypt(l3.Code))?.AccessRight);
                    level2Menus.Add(new MenuLevel2Vm
                    {
                        MenuLevel2 = l2,
                        MenuLevel3 = level3Privileges
                    });
                });

                menuPrivileges.Add(new MainMenuVm
                {
                    MainMenu = l1,
                    MenuLevel2 = level2Menus
                });
            });

            return menuPrivileges;
        }

    }

    public class MenuBuilder
    {
        private CoreDbContext _context;
        public MenuBuilder(CoreDbContext context)
        {
            _context = context;
        }

        public void BuildMenus(Controller controller, string userId, string privilegeName)
        {
            var user = _context.Users.Include(u => u.AssignedGroups).FirstOrDefault(u => u.UserID.ToUpper().Equals(userId.ToUpper()));
            var assignedGroups = user.AssignedGroups.Select(g => g.Group.ToUpper()).ToList();
            var userGroups = _context.UserGroups.Include(g => g.UserPrivileges).Where(g => assignedGroups.Contains(g.Name.ToUpper())).ToList();

            var userPrivileges = new List<UserPrivilege>();
            foreach (var userGroup in userGroups)
                userPrivileges.AddRange(userGroup.UserPrivileges);

            var higherPrivileges = userPrivileges.Where(p => p.AccessRight != AccessRight.Hidden)
            .GroupBy(p => p.PrivilegeCode)
            .Select(s => s
                .OrderByDescending(a => a.AccessRight)
                .First())
                .ToList();

            var privileges = PrivilegeBuilder.GetPrivileges(higherPrivileges, GroupOperation.MenuDisplay);
            controller.ViewBag.menus = privileges;

            var privilegeCode = ArrValues.Privileges.FirstOrDefault(p => p.Name == privilegeName)?.Code ?? "";
            var accessRight = higherPrivileges.FirstOrDefault(p => p.PrivilegeCode == Decryptor.Encrypt(privilegeCode))?.AccessRight ?? AccessRight.Read;
            controller.ViewBag.hasEditRight = accessRight == AccessRight.Write;
        }
    }
}
