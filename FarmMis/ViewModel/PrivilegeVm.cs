using FarmMis.Models;

namespace FarmMis.ViewModel
{
    public class PrivilegeVm
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Controller { get; set; }
        public string? Action { get; set; }
        public string? ParentCode { get; set; }
        public bool HasChild { get; set; }
        public AccessRight? AccessRight { get; set; }
    }

    public class UsergroupVm
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool Closed { get; set; }
        public List<MainMenuVm>? Menus { get; set; }
    }

    public class MainMenuVm
    {
        public PrivilegeVm? MainMenu { get; set; }
        public List<MenuLevel2Vm>? MenuLevel2 { get; set; }
    }

    public class MenuLevel2Vm
    {
        public PrivilegeVm? MenuLevel2 { get; set; }
        public List<PrivilegeVm>? MenuLevel3 { get; set; }
    }

    public enum GroupOperation
    {
        GroupEditing = 1,
        MenuDisplay = 2,
    }
}
