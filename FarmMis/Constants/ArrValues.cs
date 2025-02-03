using FarmMis.ViewModel;

namespace FarmMis.Constants
{
    public class StrValues
    {
		static public string UserId = "UserId";
		static public string TLS = "TLS";
        static public string SSL = "SSL";
        static public string NONE = "NONE";
        static public int PageSize = 20;
    }

    public class ArrValues
    {
        public static string[] SocketOptions = new string[] { StrValues.TLS, StrValues.SSL, StrValues.NONE };

        //public static string[] DayTimes = new string[] { "AM", "PM" };
        public static PrivilegeVm[] Privileges = new PrivilegeVm[] {
            new PrivilegeVm {
                Code = "10",
                Name = "Administration",
                ParentCode = "",
            },
            new PrivilegeVm {
                Code = "1001",
                Name = "System Setup",
                ParentCode = "10",
                Controller = "Admin",
                Action = "SysSetup",
            },
            new PrivilegeVm {
                Code = "1003",
                Name = "User Group",
                ParentCode = "10",
                Controller = "Admin",
                Action = "ListUserGroups",
            },
            new PrivilegeVm {
                Code = "1004",
                Name = "Users",
                ParentCode = "10",
                Controller = "Admin",
                Action = "ListUsers",
            },
            
            new PrivilegeVm {
                Code = "11",
                Name = "Packhouse",
                ParentCode = "",
            },
            new PrivilegeVm {
                Code = "1101",
                Name = "Packlist",
                ParentCode = "11",
                Controller = "Packhouse",
                Action = "Packlist",
            },
            //new PrivilegeVm {
            //    Code = "1102",
            //    Name = "PostScannedPacklist",
            //    ParentCode = "11",
            //    Controller = "Packhouse",
            //    Action = "PostScannedPacklist",
            //},
            //new PrivilegeVm {
            //    Code = "1101",
            //    Name = "Utilities",
            //    ParentCode = "11",
            //},
            //new PrivilegeVm {
            //    Code = "110101",
            //    Name = "HR Setup",
            //    ParentCode = "1101",
            //    Controller = "Hr",
            //    Action = "HrSetup",
            //},
        };
    }
}
