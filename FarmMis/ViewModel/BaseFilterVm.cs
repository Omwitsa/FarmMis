namespace AAAErp.ViewModel
{
    public class BaseFilterVm
    {
        public virtual int Page { get; set; }
        public virtual bool InActive { get; set; }
    }

    public class FilterUsersVm : BaseFilterVm
    {
        public string? Site { get; set; }
        public string? UserID { get; set; }
    }

    public class FilterUserGroupsVm : BaseFilterVm
    {
        public string? UserGroup { get; set; }
    }
}
