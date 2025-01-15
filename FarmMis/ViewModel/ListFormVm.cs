using FarmMis.Models;

namespace FarmMis.ViewModel
{
    public class ListFormVm
    {
        
    }

    public class UserListFormVm
    {
        public IEnumerable<User> List { get; set; }
        public FilterUsersVm Filter { get; set; }
    }

    public class UsergroupListFormVm
    {
        public IEnumerable<UserGroup> List { get; set; }
        public FilterUserGroupsVm Filter { get; set; }
    }
}
