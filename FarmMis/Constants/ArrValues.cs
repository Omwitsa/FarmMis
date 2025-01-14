using AAAErp.ViewModel;

namespace AAAErp.Constants
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
                Code = "1002",
                Name = "Site Setup",
                ParentCode = "10",
                Controller = "Admin",
                Action = "ListSites",
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
                Code = "1005",
                Name = "Backup Attendance",
                ParentCode = "10",
                Controller = "Admin",
                Action = "Ingress",
            },
            //new PrivilegeVm {
            //    Code = "11",
            //    Name = "Finance",
            //    ParentCode = "",
            //},
            //new PrivilegeVm {
            //    Code = "1101",
            //    Name = "Account Receivables",
            //    ParentCode = "11",
            //},
            //new PrivilegeVm {
            //    Code = "1102",
            //    Name = "Account Payables",
            //    ParentCode = "11",
            //},
            //new PrivilegeVm {
            //    Code = "110201",
            //    Name = "Petty Cash",
            //    ParentCode = "1102",
            //},

            new PrivilegeVm {
                Code = "12",
                Name = "Hr Management",
                ParentCode = "",
            },
            // Utility should be the last item on a menu
            new PrivilegeVm {
                Code = "1220",
                Name = "Utilities",
                ParentCode = "12",
            },
            new PrivilegeVm {
                Code = "122001",
                Name = "HR Setup",
                ParentCode = "1220",
                Controller = "Hr",
                Action = "HrSetup",
            },
        };
    }
}
