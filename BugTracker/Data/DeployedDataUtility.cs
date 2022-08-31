using BugTracker.Models;
using BugTracker.Models.Enums;
using BugTracker.Models.Settings;
using BugTracker.Services;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;

namespace BugTracker.Data
{

    public class DeployedDataUtility
    {
        #region Properties
        private readonly AppSettings _appSettings;
        private readonly ISecretsService _secretsService;
        //Company Ids
        private int company1Id;
        #endregion

        #region Constructor
        public DeployedDataUtility(IOptions<AppSettings> appSettings, ISecretsService secretsService)
        {
            _appSettings = appSettings.Value;
            _secretsService = secretsService;
        }
        #endregion

        #region Manage Data Async
        public async Task ManageDataAsync(IHost host)
        {
            using var svcScope = host.Services.CreateScope();
            var svcProvider = svcScope.ServiceProvider;
            //Service: An instance of RoleManager
            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();
            //Service: An instance of RoleManager
            var roleManagerSvc = svcProvider.GetRequiredService<RoleManager<IdentityRole>>();
            //Service: An instance of the UserManager
            var userManagerSvc = svcProvider.GetRequiredService<UserManager<BTUser>>();
            //Migration: This is the programmatic equivalent to Update-Database
            await dbContextSvc.Database.MigrateAsync();


            //Custom  Bug Tracker Seed Methods
            await SeedRolesAsync(roleManagerSvc);
            await SeedDefaultCompaniesAsync(dbContextSvc);
            await SeedDefaultUsersAsync(userManagerSvc);
            await SeedDemoUsersAsync(userManagerSvc);
            await SeedDefaultTicketTypeAsync(dbContextSvc);
            await SeedDefaultTicketStatusAsync(dbContextSvc);
            await SeedDefaultTicketPriorityAsync(dbContextSvc);
            await SeedDefaultProjectPriorityAsync(dbContextSvc);
            //await SeedDefautProjectsAsync(dbContextSvc);
            //await SeedDefautTicketsAsync(dbContextSvc);
        }
        #endregion

        #region Seed Roles Async
        public async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Roles.SiteAdmin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.ProjectManager.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Developer.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Submitter.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.DemoUser.ToString()));
        }
        #endregion

        #region Seed Default Companies Async
        public async Task SeedDefaultCompaniesAsync(ApplicationDbContext context)
        {
            try
            {
                IList<Company> defaultcompanies = new List<Company>() {
                    new Company() { Name = "Company1", Description="This is default Company 1" },
                };

                var dbCompanies = context.Companies.Select(c => c.Name).ToList();
                await context.Companies.AddRangeAsync(defaultcompanies.Where(c => !dbCompanies.Contains(c.Name)));
                await context.SaveChangesAsync();

                //Get company Ids
                company1Id = context.Companies.FirstOrDefault(p => p.Name == "Company1").Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Companies.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }
        #endregion

        #region Seed Default Project Priority Async
        public async Task SeedDefaultProjectPriorityAsync(ApplicationDbContext context)
        {
            try
            {
                IList<Models.ProjectPriority> projectPriorities = new List<ProjectPriority>() {
                                                    new ProjectPriority() { Name = BTProjectPriority.Low.ToString() },
                                                    new ProjectPriority() { Name = BTProjectPriority.Medium.ToString() },
                                                    new ProjectPriority() { Name = BTProjectPriority.High.ToString() },
                                                    new ProjectPriority() { Name = BTProjectPriority.Urgent.ToString() },
                };

                var dbProjectPriorities = context.ProjectPriorities.Select(c => c.Name).ToList();
                await context.ProjectPriorities.AddRangeAsync(projectPriorities.Where(c => !dbProjectPriorities.Contains(c.Name)));
                await context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Project Priorities.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }
        #endregion

        #region Seed Default Projects Async
        public async Task SeedDefautProjectsAsync(ApplicationDbContext context)
        {

            //Get project priority Ids
            int priorityLow = context.ProjectPriorities.FirstOrDefault(p => p.Name == BTProjectPriority.Low.ToString()).Id;
            int priorityMedium = context.ProjectPriorities.FirstOrDefault(p => p.Name == BTProjectPriority.Medium.ToString()).Id;
            int priorityHigh = context.ProjectPriorities.FirstOrDefault(p => p.Name == BTProjectPriority.High.ToString()).Id;
            int priorityUrgent = context.ProjectPriorities.FirstOrDefault(p => p.Name == BTProjectPriority.Urgent.ToString()).Id;

            try
            {
                IList<Project> projects = new List<Project>() {
                     new Project()
                     {
                         //CompanyId = company1Id,
                         //Name = "Build a Personal Porfolio",
                         //Description="Single page html, css & javascript page.  Serves as a landing page for candidates and contains a bio and links to all applications and challenges." ,
                         //StartDate = new DateTime(2021,8,20),
                         //EndDate = new DateTime(2021,8,20).AddMonths(1),
                         //ProjectPriorityId = priorityLow
                     }
                };

                var dbProjects = context.Projects.Select(c => c.Name).ToList();
                await context.Projects.AddRangeAsync(projects.Where(c => !dbProjects.Contains(c.Name)));
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Projects.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }
        #endregion

        #region Seed Default Users Async
        public async Task SeedDefaultUsersAsync(UserManager<BTUser> userManager)
        {
            //Seed Default Site Admin User
            var defaultUser = new BTUser
            {
                UserName = _secretsService.GetSiteAdminEmail(),
                Email = _secretsService.GetSiteAdminEmail(),
                FirstName = _appSettings.SiteAdminCredentials.FirstName,
                LastName = _appSettings.SiteAdminCredentials.LastName,
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, _secretsService.GetSiteAdminPassword());
                    await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.SiteAdmin.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Site Admin User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }

            //Seed Default Admin User
            defaultUser = new BTUser
            {
                UserName = _secretsService.GetDefaultUserEmail(),
                Email = _secretsService.GetDefaultUserEmail(),
                FirstName = _appSettings.DefaultCredentials.FirstName,
                LastName = _appSettings.DefaultCredentials.LastName,
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, _secretsService.GetDefaultUserPassword());
                    await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Admin User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }

        }
        #endregion

        #region Seed Demo Users Async
        public async Task SeedDemoUsersAsync(UserManager<BTUser> userManager)
        {
            //Seed Demo Admin User
            var defaultUser = new BTUser
            {
                UserName = "demoadmin@bugtracker.com",
                Email = "demoadmin@bugtracker.com",
                FirstName = "Demo",
                LastName = "Admin",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.DemoUser.ToString());

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Demo Admin User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Demo ProjectManager User
            defaultUser = new BTUser
            {
                UserName = "demopm@bugtracker.com",
                Email = "demopm@bugtracker.com",
                FirstName = "Demo",
                LastName = "ProjectManager",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.ProjectManager.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.DemoUser.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Demo ProjectManager1 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Demo Developer User
            defaultUser = new BTUser
            {
                UserName = "demodev@bugtracker.com",
                Email = "demodev@bugtracker.com",
                FirstName = "Demo",
                LastName = "Developer",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.DemoUser.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Demo Developer1 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Demo Submitter User
            defaultUser = new BTUser
            {
                UserName = "demosub@bugtracker.com",
                Email = "demosub@bugtracker.com",
                FirstName = "Demo",
                LastName = "Submitter",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Submitter.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.DemoUser.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Demo Submitter User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }
        #endregion

        #region Seed Default Ticket Type Async
        public async Task SeedDefaultTicketTypeAsync(ApplicationDbContext context)
        {
            try
            {
                IList<TicketType> ticketTypes = new List<TicketType>() {
                     new TicketType() { Name = BTTicketType.NewDevelopment.ToString() },      // Ticket involves development of a new, uncoded solution 
                     new TicketType() { Name = BTTicketType.WorkTask.ToString() },            // Ticket involves development of the specific ticket description 
                     new TicketType() { Name = BTTicketType.Defect.ToString()},               // Ticket involves unexpected development/maintenance on a previously designed feature/functionality
                     new TicketType() { Name = BTTicketType.ChangeRequest.ToString() },       // Ticket involves modification development of a previously designed feature/functionality
                     new TicketType() { Name = BTTicketType.Enhancement.ToString() },         // Ticket involves additional development on a previously designed feature or new functionality
                     new TicketType() { Name = BTTicketType.GeneralTask.ToString() }          // Ticket involves no software development but may involve tasks such as configuations, or hardware setup
                };

                var dbTicketTypes = context.TicketTypes.Select(c => c.Name).ToList();
                await context.TicketTypes.AddRangeAsync(ticketTypes.Where(c => !dbTicketTypes.Contains(c.Name)));
                await context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Ticket Types.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }
        #endregion

        #region Seed Default Ticket Status Async
        public async Task SeedDefaultTicketStatusAsync(ApplicationDbContext context)
        {
            try
            {
                IList<TicketStatus> ticketStatuses = new List<TicketStatus>() {
                    new TicketStatus() { Name = BTTicketStatus.New.ToString() },                 // Newly Created ticket having never been assigned
                    new TicketStatus() { Name = BTTicketStatus.Development.ToString() },         // Ticket is assigned and currently being worked 
                    new TicketStatus() { Name = BTTicketStatus.Testing.ToString()  },            // Ticket is assigned and is currently being tested
                    new TicketStatus() { Name = BTTicketStatus.Resolved.ToString()  },           // Ticket remains assigned to the developer but work in now complete
                };

                var dbTicketStatuses = context.TicketStatuses.Select(c => c.Name).ToList();
                await context.TicketStatuses.AddRangeAsync(ticketStatuses.Where(c => !dbTicketStatuses.Contains(c.Name)));
                await context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Ticket Statuses.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }
        #endregion

        #region Seed Default Ticket Priority Async
        public async Task SeedDefaultTicketPriorityAsync(ApplicationDbContext context)
        {
            try
            {
                IList<TicketPriority> ticketPriorities = new List<TicketPriority>() {
                                                    new TicketPriority() { Name = BTTicketPriority.Low.ToString()  },
                                                    new TicketPriority() { Name = BTTicketPriority.Medium.ToString() },
                                                    new TicketPriority() { Name = BTTicketPriority.High.ToString()},
                                                    new TicketPriority() { Name = BTTicketPriority.Urgent.ToString()},
                };

                var dbTicketPriorities = context.TicketPriorities.Select(c => c.Name).ToList();
                await context.TicketPriorities.AddRangeAsync(ticketPriorities.Where(c => !dbTicketPriorities.Contains(c.Name)));
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Ticket Priorities.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }
        #endregion

        #region Seed Default Tickets Async
        public async Task SeedDefautTicketsAsync(ApplicationDbContext context)
        {
            //Get project Ids
            int portfolioId = context.Projects.FirstOrDefault(p => p.Name == "Build a Personal Porfolio").Id;
            int blogId = context.Projects.FirstOrDefault(p => p.Name == "Build a supplemental Blog Web Application").Id;
            int bugtrackerId = context.Projects.FirstOrDefault(p => p.Name == "Build an Issue Tracking Web Application").Id;
            int movieId = context.Projects.FirstOrDefault(p => p.Name == "Build a Movie Information Web Application").Id;

            //Get ticket type Ids
            int typeNewDev = context.TicketTypes.FirstOrDefault(p => p.Name == BTTicketType.NewDevelopment.ToString()).Id;
            int typeWorkTask = context.TicketTypes.FirstOrDefault(p => p.Name == BTTicketType.WorkTask.ToString()).Id;
            int typeDefect = context.TicketTypes.FirstOrDefault(p => p.Name == BTTicketType.Defect.ToString()).Id;
            int typeEnhancement = context.TicketTypes.FirstOrDefault(p => p.Name == BTTicketType.Enhancement.ToString()).Id;
            int typeChangeRequest = context.TicketTypes.FirstOrDefault(p => p.Name == BTTicketType.ChangeRequest.ToString()).Id;

            //Get ticket priority Ids
            int priorityLow = context.TicketPriorities.FirstOrDefault(p => p.Name == BTTicketPriority.Low.ToString()).Id;
            int priorityMedium = context.TicketPriorities.FirstOrDefault(p => p.Name == BTTicketPriority.Medium.ToString()).Id;
            int priorityHigh = context.TicketPriorities.FirstOrDefault(p => p.Name == BTTicketPriority.High.ToString()).Id;
            int priorityUrgent = context.TicketPriorities.FirstOrDefault(p => p.Name == BTTicketPriority.Urgent.ToString()).Id;

            //Get ticket status Ids
            int statusNew = context.TicketStatuses.FirstOrDefault(p => p.Name == BTTicketStatus.New.ToString()).Id;
            int statusDev = context.TicketStatuses.FirstOrDefault(p => p.Name == BTTicketStatus.Development.ToString()).Id;
            int statusTest = context.TicketStatuses.FirstOrDefault(p => p.Name == BTTicketStatus.Testing.ToString()).Id;
            int statusResolved = context.TicketStatuses.FirstOrDefault(p => p.Name == BTTicketStatus.Resolved.ToString()).Id;


            try
            {
                IList<Ticket> tickets = new List<Ticket>() {
                                //PORTFOLIO
                                new Ticket() {Title = "Portfolio Ticket 1", Description = "Ticket details for portfolio ticket 1", Created = DateTimeOffset.Now, ProjectId = portfolioId, TicketPriorityId = priorityLow, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                //BLOG
                                new Ticket() {Title = "Blog Ticket 1", Description = "Ticket details for blog ticket 1", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityLow, TicketStatusId = statusNew, TicketTypeId = typeDefect},

                                //BUGTRACKER                                                                                                                         
                                new Ticket() {Title = "Bug Tracker Ticket 1", Description = "Ticket details for bug tracker ticket 1", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},

                                //MOVIE
                                new Ticket() {Title = "Movie Ticket 1", Description = "Ticket details for movie ticket 1", Created = DateTimeOffset.Now, ProjectId = movieId, TicketPriorityId = priorityLow, TicketStatusId = statusNew, TicketTypeId = typeDefect},

                };


                var dbTickets = context.Tickets.Select(c => c.Title).ToList();
                await context.Tickets.AddRangeAsync(tickets.Where(c => !dbTickets.Contains(c.Title)));
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Tickets.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        } 
        #endregion

    }
}
