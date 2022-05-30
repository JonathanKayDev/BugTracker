using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTracker.Models.ViewModels
{
    public class ManageUserRolesViewModel
    {
        public BTUser BTUser { get; set; }
        public SelectList Roles { get; set; }
        public string SelectedRole { get; set; }
    }
}
