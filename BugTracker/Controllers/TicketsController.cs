﻿#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTracker.Data;
using BugTracker.Models;
using Microsoft.AspNetCore.Identity;
using BugTracker.Extensions;
using BugTracker.Models.Enums;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using BugTracker.Models.ViewModels;

namespace BugTracker.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        #region Properties
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTProjectService _projectService;
        private readonly IBTLookupService _lookupService;
        private readonly IBTTicketService _ticketService;
        private readonly IBTFileService _fileService;
        private readonly IBTTicketHistoryService _ticketHistoryService;
        private readonly IBTRolesService _rolesService;
        #endregion

        #region Constructor
        public TicketsController(UserManager<BTUser> userManager,
                            IBTProjectService projectService,
                            IBTLookupService lookupService,
                            IBTTicketService ticketService,
                            IBTFileService fileService,
                            IBTTicketHistoryService ticketHistoryService, 
                            IBTRolesService rolesService)
        {
            _userManager = userManager;
            _projectService = projectService;
            _lookupService = lookupService;
            _ticketService = ticketService;
            _fileService = fileService;
            _ticketHistoryService = ticketHistoryService;
            _rolesService = rolesService;
        }
        #endregion

        #region My Tickets
        //GET: MyTickets
        public async Task<IActionResult> MyTickets()
        {
            BTUser bTUser = new();
            List<Ticket> tickets = new();

            // If demo user, then use a developer in company as an example for view
            if (User.IsInRole(nameof(Roles.DemoUser)))
            {
                int companyId = User.Identity.GetCompanyId().Value;
                bTUser = (await _rolesService.GetUsersInRoleAsync(nameof(Roles.Developer), companyId)).Where(u => u.FirstName != "Demo").FirstOrDefault();
                tickets = await _ticketService.GetTicketsByUserIdAsync(bTUser.Id, bTUser.CompanyId);
                // Don't allow demo user to see unassigned tickets if company Developer is also Company Admin
                tickets = tickets.Where(t => t.DeveloperUser == bTUser).ToList();
            }
            else
            {
                bTUser = await _userManager.GetUserAsync(User);
                tickets = await _ticketService.GetTicketsByUserIdAsync(bTUser.Id, bTUser.CompanyId);
            }

            

            return View(tickets);
        }
        #endregion

        #region All Tickets
        //GET: AllTickets
        public async Task<IActionResult> AllTickets()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            List<Ticket> tickets = await _ticketService.GetAllTicketsByCompanyAsync(companyId);

            // Filter list of tickets
            if (User.IsInRole(nameof(Roles.Developer)) || User.IsInRole(nameof(Roles.Submitter)))
            {
                return View(tickets.Where(t => t.Archived == false));
            }
            else
            {
                return View(tickets);
            }
        }
        #endregion

        #region Archived Tickets
        //GET: ArchivedTickets
        public async Task<IActionResult> ArchivedTickets()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            List<Ticket> tickets = await _ticketService.GetArchivedTicketsAsync(companyId);

            return View(tickets);

        }
        #endregion

        #region Unassigned Tickets
        //GET: UnassignedTickets
        [Authorize(Roles="Admin, ProjectManager")]
        public async Task<IActionResult> UnassignedTickets()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            string btUserId = _userManager.GetUserId(User);

            List<Ticket> tickets = await _ticketService.GetUnassignedTicketsAsync(companyId);

            if (User.IsInRole(nameof(Roles.Admin)))
            {
                return View(tickets);
            }
            else
            {
                // filter so PM can only view tickets for projects they are PM of
                List<Ticket> pmTickets = new();

                foreach (Ticket ticket in tickets)
                {
                    if (await _projectService.IsAssignedProjectManagerAsync(btUserId, ticket.ProjectId))
                    {
                        pmTickets.Add(ticket);
                    }
                }

                return View(pmTickets);
            }

        }
        #endregion

        #region Assign Developer
        //GET: AssignDeveloper
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> AssignDeveloper(int id)
        {
            AssignDeveloperViewModel model = new();

            model.Ticket = await _ticketService.GetTicketByIdAsync(id);
            model.Developers = new SelectList(await _projectService.GetProjectMembersByRoleAsync(model.Ticket.ProjectId,nameof(Roles.Developer)),
                                                "Id","FullName");

            return View(model);
        }

        //POST: AssignDeveloper
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignDeveloper(AssignDeveloperViewModel model)
        {
            // Check for Demo User
            if (await IsDemoUser())
            {
                return new RedirectResult("~/Identity/Account/AccessDenied");
            }

            if (model.DeveloperId != null)
            {
                BTUser btUser = await _userManager.GetUserAsync(User);
                Ticket oldTicket = await _ticketService.GetTicketAsNoTrackingAsync(model.Ticket.Id);

                try
                {
                    await _ticketService.AssignTicketAsync(model.Ticket.Id, model.DeveloperId);
                      
                }
                catch (Exception)
                {

                    throw;
                }

                // Ticket history
                Ticket newTicket = await _ticketService.GetTicketAsNoTrackingAsync(model.Ticket.Id);
                await _ticketHistoryService.AddHistoryAsync(oldTicket, newTicket, btUser.Id);

                return RedirectToAction(nameof(Details), new { id = model.Ticket.Id });
            }

            return RedirectToAction(nameof(AssignDeveloper), new {id = model.Ticket.Id});
        }
        #endregion

        #region Details
        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket ticket = await _ticketService.GetTicketByIdAsync(id.Value);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }
        #endregion

        #region Create
        // GET: Tickets/Create
        public async Task<IActionResult> Create()
        {
            BTUser btUser = await _userManager.GetUserAsync(User);
            int companyId = User.Identity.GetCompanyId().Value;

            if (User.IsInRole(nameof(Roles.Admin)))
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompany(companyId), "Id", "Name");
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetUserProjectsAsync(btUser.Id), "Id", "Name");
            }

            ViewData["TicketPriorityId"] = new SelectList(await _lookupService.GetTicketPrioritiesAsync(), "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(await _lookupService.GetTicketTypesAsync(), "Id", "Name");

            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ProjectId,TicketTypeId,TicketPriorityId")] Ticket ticket)
        {
            BTUser btUser = await _userManager.GetUserAsync(User);

            // Check for Demo User
            if(await IsDemoUser())
            {
                return new RedirectResult("~/Identity/Account/AccessDenied");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ticket.Created = DateTimeOffset.Now;
                    ticket.OwnerUserId = btUser.Id;
                    ticket.TicketStatusId = (await _ticketService.LookupTicketStatusIdAsync(nameof(BTTicketStatus.New))).Value;

                    await _ticketService.AddNewTicketAsync(ticket);

                    // Ticket history
                    Ticket newTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id);
                    await _ticketHistoryService.AddHistoryAsync(null, newTicket, btUser.Id);

                    // TODO - ticket notification
                }
                catch (Exception)
                {

                    throw;
                }

                return RedirectToAction(nameof(AllTickets));
            }

            // If model state not valid
            if (User.IsInRole(nameof(Roles.Admin)))
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompany(btUser.CompanyId), "Id", "Name");
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetUserProjectsAsync(btUser.Id), "Id", "Name");
            }

            ViewData["TicketPriorityId"] = new SelectList(await _lookupService.GetTicketPrioritiesAsync(), "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(await _lookupService.GetTicketTypesAsync(), "Id", "Name");
            return View(ticket);
        }
        #endregion

        #region Edit
        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket ticket = await _ticketService.GetTicketByIdAsync(id.Value);

            if (ticket == null)
            {
                return NotFound();
            }

            ViewData["TicketPriorityId"] = new SelectList(await _lookupService.GetTicketPrioritiesAsync(), "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(await _lookupService.GetTicketStatusesAsync(), "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(await _lookupService.GetTicketTypesAsync(), "Id", "Name", ticket.TicketTypeId);

            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Created,Updated,Archived,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,DeveloperUserId")] Ticket ticket)
        {
            // Check for Demo User
            if (await IsDemoUser())
            {
                return new RedirectResult("~/Identity/Account/AccessDenied");
            }

            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                BTUser btUser = await _userManager.GetUserAsync(User);

                Ticket oldTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id); //without notracking, model would update with the Db changes. This notracking gives us a snapshot

                try
                {
                    ticket.Updated = DateTimeOffset.Now;
                    await _ticketService.UpdateTicketAsync(ticket);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // TODO - Add ticket hitory
                Ticket newTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id);
                await _ticketHistoryService.AddHistoryAsync(oldTicket, newTicket, btUser.Id);

                return RedirectToAction(nameof(AllTickets));
            }

            ViewData["TicketPriorityId"] = new SelectList(await _lookupService.GetTicketPrioritiesAsync(), "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(await _lookupService.GetTicketStatusesAsync(), "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(await _lookupService.GetTicketTypesAsync(), "Id", "Name", ticket.TicketTypeId);

            return View(ticket);
        }
        #endregion

        #region Add Ticket Comment
        // POST: Tickets/AddTicketComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicketComment([Bind("Id,TicketId,Comment")] TicketComment ticketComment)
        {
            // Check for Demo User
            if (await IsDemoUser())
            {
                return new RedirectResult("~/Identity/Account/AccessDenied");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ticketComment.UserId = _userManager.GetUserId(User);
                    ticketComment.Created = DateTimeOffset.Now;

                    await _ticketService.AddTicketCommentAsync(ticketComment);

                    // Ticket history
                    await _ticketHistoryService.AddHistoryAsync(ticketComment.TicketId, nameof(TicketComment), ticketComment.UserId);
                }
                catch (Exception)
                {

                    throw;
                }
            }

            return RedirectToAction("Details", new { id = ticketComment.TicketId });
        }
        #endregion

        #region Add Ticket Attachment
        // POST: Tickets/AddTicketAttachment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicketAttachment([Bind("Id,FormFile,Description,TicketId")] TicketAttachment ticketAttachment)
        {
            // Check for Demo User
            if (await IsDemoUser())
            {
                return new RedirectResult("~/Identity/Account/AccessDenied");
            }

            string statusMessage;

            if (ModelState.IsValid && ticketAttachment.FormFile != null)
            {
                try
                {
                    ticketAttachment.FileData = await _fileService.ConvertFileToByteArrayAsync(ticketAttachment.FormFile);
                    ticketAttachment.FileName = ticketAttachment.FormFile.FileName;
                    ticketAttachment.FileType = ticketAttachment.FormFile.ContentType;

                    ticketAttachment.Created = DateTimeOffset.Now;
                    ticketAttachment.UserId = _userManager.GetUserId(User);

                    await _ticketService.AddTicketAttachmentAsync(ticketAttachment);
                    // Ticket history
                    await _ticketHistoryService.AddHistoryAsync(ticketAttachment.TicketId, nameof(TicketAttachment), ticketAttachment.UserId);
                }
                catch (Exception)
                {

                    throw;
                }

                statusMessage = "Success: New attachment added to Ticket.";
            }
            else
            {
                statusMessage = "Error: Invalid data.";

            }

            return RedirectToAction("Details", new { id = ticketAttachment.TicketId, message = statusMessage });
        }
        #endregion

        #region Archive
        // GET: Tickets/Archive/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket ticket = await _ticketService.GetTicketByIdAsync(id.Value);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Archive/5
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

            Ticket ticket = await _ticketService.GetTicketByIdAsync(id);
            await _ticketService.ArchiveTicketAsync(ticket);

            return RedirectToAction(nameof(AllTickets));
        }
        #endregion

        #region Restore
        // GET: Tickets/Restore/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> Restore(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket ticket = await _ticketService.GetTicketByIdAsync(id.Value);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Restore/5
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

            Ticket ticket = await _ticketService.GetTicketByIdAsync(id);
            await _ticketService.RestoreTicketAsync(ticket);

            return RedirectToAction(nameof(AllTickets));
        }
        #endregion

        #region Show File
        public async Task<IActionResult> ShowFile(int id)
        {
            TicketAttachment ticketAttachment = await _ticketService.GetTicketAttachmentByIdAsync(id);
            string fileName = ticketAttachment.FileName;
            byte[] fileData = ticketAttachment.FileData;
            string ext = Path.GetExtension(fileName).Replace(".", "");

            Response.Headers.Add("Content-Disposition", $"inline; filename={fileName}");
            return File(fileData, $"application/{ext}");
        }
        #endregion

        #region Ticket Exists
        private async Task<bool> TicketExists(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            return (await _ticketService.GetAllTicketsByCompanyAsync(companyId)).Any(t => t.Id == id);
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
