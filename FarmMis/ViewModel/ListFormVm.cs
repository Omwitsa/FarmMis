using AAAErp.Models;

namespace AAAErp.ViewModel
{
    public class ListFormVm
    {
        
    }

    public class UserListFormVm
    {
        public IEnumerable<User> List { get; set; }
        public FilterUsersVm Filter { get; set; }
    }

    public class SiteListFormVm
    {
        public IEnumerable<Site> List { get; set; }
        public FilterSitesVm Filter { get; set; }
    }

    public class UsergroupListFormVm
    {
        public IEnumerable<UserGroup> List { get; set; }
        public FilterUserGroupsVm Filter { get; set; }
    }
}
