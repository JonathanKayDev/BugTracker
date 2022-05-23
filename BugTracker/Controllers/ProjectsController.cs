#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Extensions;
using BugTracker.Models.ViewModels;
using BugTracker.Services.Interfaces;
using BugTracker.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BugTracker.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        #region Properties
        private readonly IBTRolesService _rolesService;
        private readonly IBTLookupService _lookupService;
        private readonly IBTFileService _fileService;
        private readonly IBTProjectService _projectService;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTCompanyInfoService _companyInfoService;
        #endregion

        #region Constructor
        public ProjectsController(IBTRolesService rolesService,
                            IBTLookupService lookupService,
                            IBTFileService fileService,
                            IBTProjectService projectService,
                            UserManager<BTUser> userManager,
                            IBTCompanyInfoService companyInfoService)
        {
            _rolesService = rolesService;
            _lookupService = lookupService;
            _fileService = fileService;
            _projectService = projectService;
            _userManager = userManager;
            _companyInfoService = companyInfoService;
        }
        #endregion

        #region My Projects
        // GET: MyProjects
        public async Task<IActionResult> MyProjects()
        {
            string userId = _userManager.GetUserId(User);
            List<Project> projects = await _projectService.GetUserProjectsAsync(userId);
            return View(projects);
        }
        #endregion

        #region All Projects
        // GET: AllProjects
        public async Task<IActionResult> AllProjects()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            List<Project> projects = new();

            if (User.IsInRole(nameof(Roles.Admin)) || User.IsInRole(nameof(Roles.ProjectManager)))
            {
                // Data for Admins & PMs
                // this includes archived projects
                projects = await _companyInfoService.GetAllProjectsAsync(companyId);
            }
            else
            {
                // Data for Devs and Submitters
                // this does NOT include archived projects
                projects = await _projectService.GetAllProjectsByCompany(companyId);
            }

            return View(projects);
        }
        #endregion

        #region Archived Projects
        // GET: ArchivedProjects
        public async Task<IActionResult> ArchivedProjects()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            List<Project> projects = await _projectService.GetArchivedProjectsByCompany(companyId);
            return View(projects);
        }
        #endregion

        #region Unassigned Projects
        [Authorize(Roles="Admin")]
        public async Task<IActionResult> UnassignedProjects()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            List<Project> projects = new();

            projects = await _projectService.GetUnassignedProjectsAsync(companyId);

            return View(projects);
        }
        #endregion

        #region Assign PM
        [Authorize(Roles ="Admin")]
        [HttpGet]
        // GET: Projects/AssignPM
        public async Task<IActionResult> AssignPM(int projectId)
        {
            int companyId = User.Identity.GetCompanyId().Value;
            AssignPMViewModel model = new();

            model.Project = await _projectService.GetProjectByIdAsync(projectId, companyId);
            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(Roles.ProjectManager), companyId), "Id", "FullName");

            return View(model);
        }

        // POST: Projects/AssignPM
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignPM(AssignPMViewModel model)
        {
            // Check for Demo User
            if (await IsDemoUser())
            {
                return new RedirectResult("~/Identity/Account/AccessDenied");
            }

            if (!string.IsNullOrEmpty(model.PMId))
            {
                try
                {
                    await _projectService.AddProjectManagerAsync(model.PMId, model.Project.Id);

                    return RedirectToAction(nameof(Details), new { id = model.Project.Id });
                }
                catch (Exception)
                {

                    throw;
                }
            }

            return RedirectToAction(nameof(AssignPM), new { projectId = model.Project.Id });
        }

        #endregion

        #region Assign Members
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpGet]
        // GET: Projects/AssignMembers
        public async Task<IActionResult> AssignMembers(int projectId)
        {
            ProjectMembersViewModel model = new();
            int companyId = User.Identity.GetCompanyId().Value;

            model.Project = await _projectService.GetProjectByIdAsync(projectId, companyId);

            List<BTUser> developers = await _rolesService.GetUsersInRoleAsync(nameof(Roles.Developer), companyId);
            List<BTUser> submitters = await _rolesService.GetUsersInRoleAsync(nameof(Roles.Submitter), companyId);

            List<BTUser> companyMembers = developers.Concat(submitters).ToList();
            List<string> projectMembers = model.Project.Members.Select(m=>m.Id).ToList();

            // projectMembers will be greyed out in list below
            model.Users = new MultiSelectList(companyMembers,"Id","FullName", projectMembers);

            return View(model);

        }

        // POST: Projects/AssignMembers
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignMembers(ProjectMembersViewModel model)
        {
            // Check for Demo User
            if (await IsDemoUser())
            {
                return new RedirectResult("~/Identity/Account/AccessDenied");
            }

            if (model.SelectedUsers != null)
            {
                List<string> memberIds = (await _projectService.GetAllProjectMembersExceptPMAsync(model.Project.Id))
                                                                .Select(m=>m.Id).ToList();

                // Remove current members from project
                foreach (string member in memberIds)
                {
                    await _projectService.RemoveUserFromProjectAsync(member, model.Project.Id);
                }

                // Add selected members to project
                foreach (string member in model.SelectedUsers)
                {
                    await _projectService.AddUserToProjectAsync(member, model.Project.Id);
                }

                // Go to project details
                return RedirectToAction("Details", "Projects", new { id = model.Project.Id });
            }

            return RedirectToAction(nameof(AssignMembers), new { projectId = model.Project.Id});
        }
        #endregion

        #region Details
        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int companyId = User.Identity.GetCompanyId().Value;

            Project project = await _projectService.GetProjectByIdAsync(id.Value, companyId);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }
        #endregion

        #region Create
        [Authorize(Roles = "Admin, ProjectManager")]
        // GET: Projects/Create
        public async Task<IActionResult> Create()
        {
            int companyId = User.Identity.GetCompanyId().Value;

            // Add ViewModel instance
            AddProjectWithPMViewModel model = new();

            // Load SelectLists with data, i.e. PMList & PriorityList
            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(Roles.ProjectManager.ToString(), companyId), "Id", "FullName");
            model.PriorityList = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Name");

            return View(model);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddProjectWithPMViewModel model)
        {
            // Check for Demo User
            if (await IsDemoUser())
            {
                return new RedirectResult("~/Identity/Account/AccessDenied");
            }

            if (model != null)
            {
                int companyId = User.Identity.GetCompanyId().Value;

                try
                {
                    // Add project image
                    if (model.Project.ImageFormFile != null)
                    {
                        model.Project.ImageFileData = await _fileService.ConvertFileToByteArrayAsync(model.Project.ImageFormFile);
                        model.Project.ImageFileName = model.Project.ImageFormFile.FileName;
                        model.Project.ImageFileType = model.Project.ImageFormFile.ContentType;
                    }

                    // Add companyId
                    model.Project.CompanyId = companyId;

                    // Save project to Db
                    await _projectService.AddNewProjectAsync(model.Project);

                    // Add PM if one was chosen
                    // This code is AFTER saving project so that project is given an Id with EntityFramework
                    if (!string.IsNullOrEmpty(model.PmId))
                    {
                        await _projectService.AddProjectManagerAsync(model.PmId, model.Project.Id);
                    }

                    // TODO - Redirect to All Projects
                    return RedirectToAction("AllProjects");
                }
                catch (Exception)
                {

                    throw;
                }

            }

            return RedirectToAction("Create");
        }
        #endregion

        #region Edit
        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            // Add ViewModel instance
            AddProjectWithPMViewModel model = new();

            model.Project = await _projectService.GetProjectByIdAsync(id.Value, companyId);

            // Load SelectLists with data, i.e. PMList & PriorityList
            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(Roles.ProjectManager.ToString(), companyId), "Id", "FullName");
            model.PriorityList = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Name");

            return View(model);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AddProjectWithPMViewModel model)
        {
            // Check for Demo User
            if (await IsDemoUser())
            {
                return new RedirectResult("~/Identity/Account/AccessDenied");
            }

            if (model != null)
            {
                try
                {
                    // Add project image
                    if (model.Project.ImageFormFile != null)
                    {
                        model.Project.ImageFileData = await _fileService.ConvertFileToByteArrayAsync(model.Project.ImageFormFile);
                        model.Project.ImageFileName = model.Project.ImageFormFile.FileName;
                        model.Project.ImageFileType = model.Project.ImageFormFile.ContentType;
                    }

                    // Update project to Db
                    await _projectService.UpdateProjectAsync(model.Project);

                    // Add PM if one was chosen
                    // This code is AFTER saving project so that project is given an Id with EntityFramework
                    if (!string.IsNullOrEmpty(model.PmId))
                    {
                        await _projectService.AddProjectManagerAsync(model.PmId, model.Project.Id);
                    }

                    // TODO - Redirect to All Projects
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ProjectExists(model.Project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return RedirectToAction("Edit");
        }
        #endregion

        #region Archive
        // GET: Projects/Archive/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int companyId = User.Identity.GetCompanyId().Value;
            var project = await _projectService.GetProjectByIdAsync(id.Value, companyId);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Archive/5
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {
            // Check for Demo User
            if (await IsDemoUser())
            {
                return new RedirectResult("~/Identity/Account/AccessDenied");
            }

            int companyId = User.Identity.GetCompanyId().Value;
            var project = await _projectService.GetProjectByIdAsync(id, companyId);

            await _projectService.ArchiveProjectAsync(project);

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Restore
        // GET: Projects/Restore/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> Restore(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int companyId = User.Identity.GetCompanyId().Value;
            var project = await _projectService.GetProjectByIdAsync(id.Value, companyId);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Restore/5
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {
            // Check for Demo User
            if (await IsDemoUser())
            {
                return new RedirectResult("~/Identity/Account/AccessDenied");
            }

            int companyId = User.Identity.GetCompanyId().Value;
            var project = await _projectService.GetProjectByIdAsync(id, companyId);

            await _projectService.RestoreProjectAsync(project);

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Project Exists
        private async Task<bool> ProjectExists(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            return (await _projectService.GetAllProjectsByCompany(companyId)).Any(p => p.Id == id);
        }
        #endregion

        #region Is Demo User
        private async Task<bool> IsDemoUser()
        {
            // Check if Demo User
            BTUser btUser = await _userManager.GetUserAsync(User);
            if (await _userManager.IsInRoleAsync(btUser, nameof(Roles.DemoUser)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
