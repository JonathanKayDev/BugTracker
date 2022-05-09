using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Enums;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class BTProjectService : IBTProjectService
    {
        #region Properties
        private readonly ApplicationDbContext _context;
        private readonly IBTRolesService _rolesService; // TODO - Question - why instantiate an Interface? 
        #endregion

        #region Constructor
        // Constructor
        public BTProjectService(ApplicationDbContext context, IBTRolesService rolesService)
        {
            _context = context;
            _rolesService = rolesService;
        }
        #endregion

        #region Add New Project
        // CRUD - Create
        public async Task AddNewProjectAsync(Project project)
        {
            _context.Add(project);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Add Project Manager
        public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            BTUser currentPM = await GetProjectManagerAsync(projectId);

            // Remove the current PM is necessary
            if (currentPM != null)
            {
                try
                {
                    await RemoveProjectManagerAsync(projectId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"**** ERROR **** - Error removing project manager from project. --> {ex.Message}");
                    return false;
                }
            }

            // Add the new PM
            try
            {
                //TODO - test below works - had to deviate from tutorials
                await AddUserToProjectAsync(userId, projectId);

                // Add PM to user role
                BTUser newPM = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                await _rolesService.AddUserToRoleAsync(newPM, Roles.ProjectManager.ToString());

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"**** ERROR **** - Error adding project manager to project. --> {ex.Message}");
                return false;
            }
        }
        #endregion

        #region Add User To Project
        public async Task<bool> AddUserToProjectAsync(string userId, int projectId)
        {
            BTUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user != null)
            {
                Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

                if (!await IsUserOnProjectAsync(userId, projectId))
                {
                    try
                    {
                        project.Members.Add(user);
                        await _context.SaveChangesAsync();
                        return true;
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                }
            }

            return false;
        }
        #endregion

        #region Archive Project
        // CRUD - Archive
        public async Task ArchiveProjectAsync(Project project)
        {
            try
            {
                project.Archived = true;
                await UpdateProjectAsync(project);

                // Archive the Tickets for the project
                foreach (Ticket ticket in project.Tickets)
                {
                    ticket.ArchivedByProject = true;
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Get All Project Members Except Project Manager
        public async Task<List<BTUser>> GetAllProjectMembersExceptPMAsync(int projectId)
        {
            List<BTUser> developers = await GetProjectMembersByRoleAsync(projectId, Roles.Developer.ToString());
            List<BTUser> submitters = await GetProjectMembersByRoleAsync(projectId, Roles.Submitter.ToString());
            List<BTUser> admins = await GetProjectMembersByRoleAsync(projectId, Roles.Admin.ToString());

            List<BTUser> teamMembers = developers.Concat(submitters).Concat(admins).ToList();

            return teamMembers;
        }
        #endregion

        #region Get All Projects By Company Id
        public async Task<List<Project>> GetAllProjectsByCompany(int companyId)
        {
            try
            {
                List<Project> results = await _context.Projects.Where(p => p.CompanyId == companyId && p.Archived == false)
                                                    // navigation items 'eager loaded' otherwise navigation items won't be avilable in results
                                                    .Include(p => p.Members)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.Comments)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.Attachments)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.History)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.Notifications)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.DeveloperUser)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.OwnerUser)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.TicketStatus)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.TicketPriority)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.TicketType)
                                                    .Include(p => p.ProjectPriority)
                                                    .ToListAsync();

                return results;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Get All Projects By Priority
        public async Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName)
        {
            List<Project> results = await GetAllProjectsByCompany(companyId);
            int priorityId = await LookupProjectPriorityId(priorityName);

            return results.Where(p => p.ProjectPriorityId == priorityId).ToList();
        }
        #endregion

        #region Get Archived Projects By Company
        public async Task<List<Project>> GetArchivedProjectsByCompany(int companyId)
        {
            try
            {
                List<Project> results = await _context.Projects.Where(p => p.CompanyId == companyId && p.Archived == true)
                                                    // navigation items 'eager loaded' otherwise navigation items won't be avilable in results
                                                    .Include(p => p.Members)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.Comments)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.Attachments)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.History)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.Notifications)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.DeveloperUser)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.OwnerUser)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.TicketStatus)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.TicketPriority)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.TicketType)
                                                    .Include(p => p.ProjectPriority)
                                                    .ToListAsync();

                return results;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Get Developers On Project
        public Task<List<BTUser>> GetDevelopersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Get Project By Id
        // CRUD - Read
        public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
        {
            try
            {
                Project result = await _context.Projects.Include(p => p.Tickets)
                                                    .ThenInclude(t => t.TicketPriority)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.TicketStatus)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.TicketType)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.DeveloperUser)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.OwnerUser)
                                                .Include(p => p.Members)
                                                .Include(p => p.ProjectPriority)
                                                .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Get Project Manager
        public async Task<BTUser> GetProjectManagerAsync(int projectId)
        {
            Project project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);

            foreach (BTUser member in project?.Members)
            {
                if (await _rolesService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
                {
                    return member;
                }
            }

            return null;
        }
        #endregion

        #region Get Project Members By Role
        public async Task<List<BTUser>> GetProjectMembersByRoleAsync(int projectId, string role)
        {
            Project project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);

            List<BTUser> members = new();

            foreach (var user in project.Members)
            {
                if (await _rolesService.IsUserInRoleAsync(user, role))
                {
                    members.Add(user);
                }
            }

            return members;
        }
        #endregion

        #region Get Submitters On Project
        public Task<List<BTUser>> GetSubmittersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Get User Projects
        public async Task<List<Project>> GetUserProjectsAsync(string userId)
        {
            try
            {
                List<Project> userProjects = (await _context.Users.Include(u => u.Projects)
                                                                    .ThenInclude(p => p.Company)
                                                                .Include(u => u.Projects)
                                                                    .ThenInclude(p => p.Members)
                                                                .Include(u => u.Projects)
                                                                    .ThenInclude(p => p.Tickets)
                                                                .Include(u => u.Projects)
                                                                    .ThenInclude(p => p.Tickets)
                                                                        .ThenInclude(t => t.DeveloperUser)
                                                                .Include(u => u.Projects)
                                                                    .ThenInclude(p => p.Tickets)
                                                                        .ThenInclude(t => t.OwnerUser)
                                                                .Include(u => u.Projects)
                                                                    .ThenInclude(p => p.Tickets)
                                                                        .ThenInclude(t => t.TicketPriority)
                                                                .Include(u => u.Projects)
                                                                    .ThenInclude(p => p.Tickets)
                                                                        .ThenInclude(t => t.TicketStatus)
                                                                .Include(u => u.Projects)
                                                                    .ThenInclude(p => p.Tickets)
                                                                        .ThenInclude(t => t.TicketType)
                                                                .FirstOrDefaultAsync(u => u.Id == userId)).Projects.ToList();

                return userProjects;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"**** ERROR **** - Error getting user from project list. --> {ex.Message}");
                throw;
            }
        }
        #endregion

        #region Get Users Not On Project
        public async Task<List<BTUser>> GetUsersNotOnProjectAsync(int projectId, int companyId)
        {
            List<BTUser> users = await _context.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToListAsync();

            return users.Where(u => u.CompanyId == companyId).ToList();
        }
        #endregion

        #region Is User On Project
        public async Task<bool> IsUserOnProjectAsync(string userId, int projectId)
        {
            Project project = await _context.Projects
                                            .Include(p => p.Members)
                                            .FirstOrDefaultAsync(p => p.Id == projectId);
            bool result = false;

            if (project != null)
            {
                result = project.Members.Any(m => m.Id == userId);
            }

            return result;
        }
        #endregion

        #region Lookup Project Priority Id
        public async Task<int> LookupProjectPriorityId(string priorityName)
        {
            int priorityId = (await _context.ProjectPriorities.FirstOrDefaultAsync(p => p.Name == priorityName)).Id;

            return priorityId;
        }
        #endregion

        #region Remove Project Manager
        public async Task RemoveProjectManagerAsync(int projectId)
        {
            Project project = await _context.Projects
                                            .Include(p => p.Members)
                                            .FirstOrDefaultAsync(p => p.Id == projectId);

            try
            {
                foreach (BTUser member in project?.Members)
                {
                    if (await _rolesService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
                    {
                        await RemoveUserFromProjectAsync(member.Id, projectId);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Remove User From Project
        public async Task RemoveUserFromProjectAsync(string userId, int projectId)
        {
            try
            {
                BTUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

                try
                {
                    if (await IsUserOnProjectAsync(userId, projectId))
                    {
                        project.Members.Remove(user);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"**** ERROR **** - Error removing user from project. --> {ex.Message}");
            }
        }
        #endregion

        #region Remove Users From Project By Role
        public async Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
        {
            try
            {
                List<BTUser> members = await GetProjectMembersByRoleAsync(projectId, role);
                // TODO - why doesn't project below need to include members?
                Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

                foreach (BTUser user in members)
                {
                    try
                    {
                        project.Members.Remove(user);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"**** ERROR **** - Error removing users from project. --> {ex.Message}");
                throw;
            }
        }
        #endregion

        #region Restore Project
        public async Task RestoreProjectAsync(Project project)
        {
            try
            {
                project.Archived = false;
                await UpdateProjectAsync(project);

                // Archive the Tickets for the project
                foreach (Ticket ticket in project.Tickets)
                {
                    ticket.ArchivedByProject = false;
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Update Project
        // CRUD - Update
        public async Task UpdateProjectAsync(Project project)
        {
            _context.Update(project);
            await _context.SaveChangesAsync();
        } 
        #endregion
    }
}
