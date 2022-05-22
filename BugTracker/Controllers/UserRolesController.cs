using BugTracker.Extensions;
using BugTracker.Models;
using BugTracker.Models.ViewModels;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTracker.Controllers
{
    [Authorize]
    public class UserRolesController : Controller
    {
        #region Properties
        private readonly IBTRolesService _rolesService;
        private readonly IBTCompanyInfoService _companyInfoService;
        #endregion

        #region Constructor
        public UserRolesController(IBTRolesService rolesService, IBTCompanyInfoService companyInfoService)
        {
            _rolesService = rolesService;
            _companyInfoService = companyInfoService;
        }
        #endregion

        #region Manage User Roles
        [HttpGet]
        public async Task<IActionResult> ManageUserRoles()
        {
            // Add instance of the ViewModel as a list
            List<ManageUserRolesViewModel> model = new();

            // Get CompanyId
            int companyId = User.Identity.GetCompanyId().Value;

            // Get all company users
            List<BTUser> users = await _companyInfoService.GetAllMembersAsync(companyId);

            // Loop over the users to populate the ViewModel
            // - instantiate the ViewModel
            // - user _rolesService
            // - create multi-selects
            foreach (BTUser user in users)
            {
                ManageUserRolesViewModel viewModel = new();
                viewModel.BTUser = user;
                IEnumerable<string> selected = await _rolesService.GetUserRolesAsync(user);
                viewModel.Roles = new MultiSelectList(await _rolesService.GetRolesAsync(), "Name", "Name", selected);

                model.Add(viewModel);
            }

            // Return the model to the View
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
        {
            // Get the company Id
            int companyId = User.Identity.GetCompanyId().Value;

            // Instantiate the BTUser
            BTUser user = (await _companyInfoService.GetAllMembersAsync(companyId)).FirstOrDefault(u => u.Id == member.BTUser.Id);

            // Get Roles for the user
            IEnumerable<string> roles = await _rolesService.GetUserRolesAsync(user);

            // Grab the selected role
            string userRole = member.SelectedRoles.FirstOrDefault();

            if (!string.IsNullOrEmpty(userRole))
            {
                // Remove user from their roles
                if (await _rolesService.RemoveUserFromRolesAsync(user, roles))
                {
                    // Add user to the new role
                    await _rolesService.AddUserToRoleAsync(user, userRole);
                }
            }

            // Navigate back to the view
            return RedirectToAction(nameof(ManageUserRoles));
        } 
        #endregion
    }
}
