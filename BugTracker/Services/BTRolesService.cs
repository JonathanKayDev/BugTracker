using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class BTRolesService : IBTRolesService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BTUser> _userManager;
        
        // Constructor
        public BTRolesService(ApplicationDbContext context,
                                RoleManager<IdentityRole> roleManager,
                                UserManager<BTUser> userManager)
        {
            // Dependency injection
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }


        public async Task<bool> AddUserToToleAsync(BTUser user, string roleName)
        {
            bool result = (await _userManager.AddToRoleAsync(user, roleName)).Succeeded;
            return result;
        }

        public async Task<string> GetRoleNameByIdAsync(string roleId)
        {
            IdentityRole role = _context.Roles.Find(roleId);
            string result = await _roleManager.GetRoleNameAsync(role);
            return result;
        }

        #region Get Roles
        public async Task<List<IdentityRole>> GetRolesAsync()
        {
            try
            {
                List<IdentityRole> result = new();

                result = await _context.Roles.ToListAsync();

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        } 
        #endregion

        public async Task<IEnumerable<string>> GetUserRolesAsync(BTUser user)
        {

            IEnumerable<string> results = await _userManager.GetRolesAsync(user);
            return results;
        }

        public async Task<List<BTUser>> GetUsersInRoleAsync(string roleName, int companyId)
        {
            List<BTUser> users = (await _userManager.GetUsersInRoleAsync(roleName)).ToList();
            List<BTUser> results = users.Where(u => u.CompanyId == companyId).ToList();
            return results;
        }

        public async Task<List<BTUser>> GetUsersNotInRoleAsync(string roleName, int companyId)
        {
            List<string> userIds = (await _userManager.GetUsersInRoleAsync(roleName)).Select(u => u.Id).ToList();
            List<BTUser> roleUsers = _context.Users.Where(u => !userIds.Contains(u.Id)).ToList();

            List<BTUser> results = roleUsers.Where(u => u.CompanyId == companyId).ToList();

            return results;
        }

        public async Task<bool> IsUserInRoleAsync(BTUser user, string roleName)
        {
            bool result = await _userManager.IsInRoleAsync(user, roleName);
            return result;
        }

        public async Task<bool> RemoveUserFromRoleAsync(BTUser user, string roleName)
        {
            bool result = (await _userManager.RemoveFromRoleAsync(user, roleName)).Succeeded;
            return result;
        }

        public async Task<bool> RemoveUserFromRolesAsync(BTUser user, IEnumerable<string> roles)
        {
            bool result = (await _userManager.RemoveFromRolesAsync(user, roles)).Succeeded;
            return result;
        }
    }
}
