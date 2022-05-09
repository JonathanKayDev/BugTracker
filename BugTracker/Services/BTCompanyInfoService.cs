using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class BTCompanyInfoService : IBTCompanyInfoService
    {
        #region Properties
        private readonly ApplicationDbContext _context;
        #endregion

        #region Constructor
        // Constructor
        public BTCompanyInfoService(ApplicationDbContext context)
        {
            _context = context;
        }
        #endregion

        #region Get All Members
        public async Task<List<BTUser>> GetAllMembersAsync(int companyId)
        {
            List<BTUser> results = new();

            results = await _context.Users.Where(u => u.CompanyId == companyId).ToListAsync();

            return results;
        }
        #endregion

        #region Get All Projects
        public async Task<List<Project>> GetAllProjectsAsync(int companyId)
        {
            List<Project> results = new();

            results = await _context.Projects.Where(p => p.CompanyId == companyId)
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
        #endregion

        #region Get All Tickets
        public async Task<List<Ticket>> GetAllTicketsAsync(int companyId)
        {
            List<Ticket> results = new();
            List<Project> projects = new();

            projects = await GetAllProjectsAsync(companyId);
            results = projects.SelectMany(p => p.Tickets).ToList();

            return results;
        }
        #endregion

        #region Get Company Info
        public async Task<Company> GetCompanyInfoByIdAsync(int? companyId)
        {
            Company results = new();

            if (companyId != null)
            {
                results = await _context.Companies.Include(c => c.Members)
                                                    .Include(c => c.Projects)
                                                    .Include(c => c.Invites)
                                                    .FirstOrDefaultAsync(c => c.Id == companyId);
            }

            return results;
        } 
        #endregion
    }
}
