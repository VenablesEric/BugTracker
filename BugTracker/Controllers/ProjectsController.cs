using BugTracker.Areas.Identity.Data;
using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Database;
using BugTracker.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly BugTrackerDbContext _context;
        private readonly UserManager<BugTrackerUser> _userManager;

        public ProjectsController(BugTrackerDbContext context, UserManager<BugTrackerUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult ListProjects()
        {
            var projects = _context.Projects;

            return View(new ProjectsModel
            {
                Projects = projects,
                NewProject = new Project()
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProject(ProjectsModel model)
        {
            if (ModelState.IsValid)
            {
                _context.Projects.Add(model.NewProject);
                await _context.SaveChangesAsync();

                return RedirectToAction("ListProjects", "Projects");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Project(string id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                ViewBag.ErrorMessage = $"Project with Id = {id} cannot be found";
                return RedirectToAction("ListProjects", "Projects");
                //return View("NotFound");
            }

            var users = _userManager.Users;
            if (users == null)
            {
                ViewBag.ErrorMessage = $"Something went worng";
                return RedirectToAction("ListProjects", "Projects");
                //return View("NotFound");
            }
            var projectMemberId = _context.ProjectMembers.Where(x => x.ProjectId == id);


            /*var query1 = from BugTrackerUser in users
                          join ProjectMembers in projectMemberId on BugTrackerUser
                          equals ProjectMembers.UserId into gj 
                          from subpets in gj
                          select new { }
 */
            var q2 = (from BugTrackerUser in users
                      join ProjectMembers in projectMemberId on
                      BugTrackerUser.Id equals ProjectMembers.UserId
                      select BugTrackerUser).ToList();

            var q3 = (from BugTrackerUser in users
                      join ProjectMembers in projectMemberId on
                      BugTrackerUser.Id equals ProjectMembers.UserId into ps
                      from ProjectMembers in ps.DefaultIfEmpty()
                      select BugTrackerUser).ToList();

            var q4 = users.Where(id => !q2.Contains(id)).ToList();


            var tickets2 = _context.Tickets.Where(t => t.ProjectId == id);
            var tickets = tickets2.ToList();

            if (tickets == null)
            {
                ViewBag.ErrorMessage = $"Something went worng";
                return RedirectToAction("ListProjects", "Projects");
                //return View("NotFound");
            }

            var projectData = new ProjectDataModel
            {
                Infomation = project,
                Tickets = tickets,
                Members = q2,
                NonMembers = CreateSelectListItem(q4),
                SelectedNonMembers = new List<string>(),
                NewTick = new Ticket()
            };

            return View(projectData);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AddUserToProject(ProjectDataModel model)
        {
            var project = await _context.Projects.FindAsync(model.Infomation.Id);
            if (project == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {project.Name} cannot be found";
                return RedirectToAction("Project", "Projects", new { id = project.Id });
                //return View("NotFound");
            }

            foreach (var test in model.SelectedNonMembers)
            {
                var user = await _userManager.FindByIdAsync(test);

                if (user == null)
                    continue;

                _context.ProjectMembers.Add(new ProjectMember
                {
                    UserId = user.Id,
                    ProjectId = project.Id
                });
            }
            await _context.SaveChangesAsync();
            //return View("Project", project.Id);
            return RedirectToAction("Project", "Projects", new { id = project.Id });

            /*var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var project = await _context.Projects.FindAsync(projectId);
            if (project == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {projectId} cannot be found";
                return View("NotFound");
            }

            _context.ProjectMembers.Add(new ProjectMembers
            {
                UserId = user.Id,
                ProjectId = project.Id
            });
            await _context.SaveChangesAsync();

            return View("Project", projectId);*/

        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> RemoveMember(string userId, string projectId)
        {
            var e = _context.ProjectMembers.Where(x => x.UserId == userId).Where(x => x.ProjectId == projectId).FirstOrDefault();

            if (e == null)
            {
                ViewBag.ErrorMessage = $"Project with Id = {e.ProjectId} cannot be found";
                return RedirectToAction("Project", "Projects", new { id = projectId });
                //return View("NotFound");
            }

            _context.Remove(e);
            await _context.SaveChangesAsync();

            return RedirectToAction("Project", "Projects", new { id = projectId });

        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> EditProject(ProjectDataModel model)
        {
            var project = await _context.Projects.FindAsync(model.Infomation.Id);

            if (project == null)
            {
                ViewBag.ErrorMessage = $"Project with Id = {model.Infomation.Id} cannot be found";
                return RedirectToAction("Project", "Projects", new { id = project.Id });
                //return View("NotFound");
            }

            project.Name = model.Infomation.Name;
            project.Description = model.Infomation.Description;

            await _context.SaveChangesAsync();

            return RedirectToAction("Project", "Projects", new { id = project.Id });
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteProject(string id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                ViewBag.ErrorMessage = $"Project with Id = {project.Id} cannot be found";
                return RedirectToAction("ListProjects", "Projects");
                //return View("NotFound");
            }

            _context.Remove(project);
            await _context.SaveChangesAsync();

            return RedirectToAction("ListProjects", "Projects");

        }

        [HttpGet]
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tickets = _context.Tickets.Where(t => t.AssignUserId == userId)
                .Where(t => t.Status != TICKET_STATUS.RESOLVED);

            var red = "#eb4034";
            var purple = "#b134eb";
            var yellow = "#ebe534";
            var green = "#34eb3a";
            var blue = "#3468eb";

            var dashModel = new DashModel();

            var priority = new PieChartVM();
            priority.datasets.Add(new PieChartChildVM 
            { 
                backgroundColor = {blue,green,yellow,red },
                data = 
                {
                    tickets.Where(t => t.Priority == TICKET_PRIORITY.NONE).Count(),
                    tickets.Where(t => t.Priority == TICKET_PRIORITY.LOW).Count(),
                    tickets.Where(t => t.Priority == TICKET_PRIORITY.MEDIUM).Count(),
                    tickets.Where(t => t.Priority == TICKET_PRIORITY.HIGH).Count()
                } 
            });
            priority.labels = new List<string> 
            {
                $"None: {priority.datasets[0].data[0]}", 
                $"Low: {priority.datasets[0].data[1]}", 
                $"Medium: {priority.datasets[0].data[2]}", 
                $"Hight: {priority.datasets[0].data[3]}" 
            };

            var status = new PieChartVM();
            status.datasets.Add(new PieChartChildVM
            {
                backgroundColor = { blue,green,yellow,red },
                data =
                {
                    tickets.Where(t => t.Status == TICKET_STATUS.NEW).Count(),
                    tickets.Where(t => t.Status == TICKET_STATUS.OPEN).Count(),
                    tickets.Where(t => t.Status == TICKET_STATUS.IN_PROGRESS).Count(),
                    tickets.Where(t => t.Status == TICKET_STATUS.RESOLVED).Count()
                }
            });
            status.labels = new List<string> 
            { 
                $"New: {status.datasets[0].data[0]}", 
                $"Open: {status.datasets[0].data[1]}", 
                $"In Progress: {status.datasets[0].data[2]}", 
                $"Resolved: {status.datasets[0].data[3]}"
            };

            var type = new PieChartVM();
            type.datasets.Add(new PieChartChildVM
            {
                backgroundColor = { blue,green,yellow,purple,red },
                data =
                {
                     tickets.Where(t => t.Type == TICKET_TYPE.NONE).Count(),
                     tickets.Where(t => t.Type == TICKET_TYPE.BUG).Count(),
                     tickets.Where(t => t.Type == TICKET_TYPE.ERROR).Count(),
                     tickets.Where(t => t.Type == TICKET_TYPE.FEATURE).Count(),
                     tickets.Where(t => t.Type == TICKET_TYPE.OTHER).Count()
                }
            });
            type.labels = new List<string> { 
                $"None: {type.datasets[0].data[0]}", 
                $"Bug: {type.datasets[0].data[1]}", 
                $"Error: {type.datasets[0].data[2]}", 
                $"Feature: {type.datasets[0].data[3]}", 
                $"Other: {type.datasets[0].data[4]}" 
            };

            dashModel.priority = priority;
            dashModel.status = status;
            dashModel.type = type;
            dashModel.count = tickets.Count();
            
            return View(dashModel);
        }

        private List<SelectListItem> CreateSelectListItem(List<BugTrackerUser> users)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (BugTrackerUser user in users)
            {
                items.Add(new SelectListItem(user.UserName, user.Id));
            }
            return items;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Coder,User")]
        public async Task<IActionResult> CreateTicket(ProjectDataModel model)
        {
            var project = await _context.Projects.FindAsync(model.NewTick.ProjectId);
            if (project == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {project.Name} cannot be found";
                return RedirectToAction("Project", "Projects", new { id = project.Id });
                //return View("NotFound");
            }

            /*var user = await _userManager.FindByIdAsync(model.NewTick.AuthorUserId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {project.Name} cannot be found";
                return View("NotFound");
            }*/

            string assignUserId = null;
            if (model.NewTick.AssignUserId != null)
            {
                assignUserId = (await _userManager.FindByNameAsync(model.NewTick.AssignUserId))?.Id;
            }

            _context.Tickets.Add(new Ticket
            {
                ProjectId = project.Id,
                AuthorUserId = model.NewTick.AuthorUserId,
                AssignUserId = assignUserId,
                Title = model.NewTick.Title,
                Description = model.NewTick.Description,
                Type = model.NewTick.Type,
                Priority = model.NewTick.Priority,
                Status = model.NewTick.Status,
            });

            await _context.SaveChangesAsync();
            return RedirectToAction("Project", "Projects", new { id = project.Id });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Coder,User")]
        public async Task<IActionResult> EditTicket(TicketData model)
        {
            var ticket = await _context.Tickets.FindAsync(model.Ticket.Id);

            if (ticket == null)
            {
                ViewBag.ErrorMessage = $"Project with Id = {model.Ticket.Id} cannot be found";
                return RedirectToAction("Ticket", "Projects", new { id = ticket.Id });
                //return View("NotFound");
            }

            ticket.Title = model.Ticket.Title;
            ticket.Description = model.Ticket.Description;

            if(model.Ticket.AssignUserId  == "NONE")
                ticket.AssignUserId = null;
            else
                ticket.AssignUserId = model.Ticket.AssignUserId;

            ticket.Type = model.Ticket.Type;
            ticket.Priority = model.Ticket.Priority;
            ticket.Status = model.Ticket.Status;

            await _context.SaveChangesAsync();

            return RedirectToAction("Ticket", "Projects", new { id = ticket.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Ticket(string id)
        {
            var ticket = await _context.Tickets.FindAsync(id);

            if (ticket == null)
            {
                ViewBag.ErrorMessage = $"Ticket with Id = {id} cannot be found";
                return RedirectToAction("Index", "Home");
                //return View("NotFound");
            }

            var users = _userManager.Users;
            if (users == null)
            {
                ViewBag.ErrorMessage = $"Something went worng";
                return RedirectToAction("Index", "Home");
                //return View("NotFound");
            }

            var membersId = _context.ProjectMembers.Where(x => x.ProjectId == ticket.ProjectId);
            var members = (from BugTrackerUser in users
                           join ProjectMembers in membersId on
                           BugTrackerUser.Id equals ProjectMembers.UserId
                           select BugTrackerUser).ToList();

            var comments = _context.TickertComments.Where(x => x.TicketId == ticket.Id).ToList();
            List<TicketCommentData> data = new List<TicketCommentData>();

            foreach (var comment in comments)
            {
                data.Add(new TicketCommentData
                {
                    TicketComment = new TicketComment
                    {
                        Id = comment.Id,
                        TicketId = ticket.Id,
                        AuthorId = comment.AuthorId,
                        Comment = comment.Comment
                    },
                    AuthorUserName = (await _userManager.FindByIdAsync(comment.AuthorId)).UserName
                });
            }

            var assignUserName = await _userManager.FindByIdAsync(ticket.AssignUserId);
            var authorUserName = await _userManager.FindByIdAsync(ticket.AuthorUserId);

            SelectList sl;
            if (ticket.AssignUserId == null)
                sl = new SelectList(members, "Id", "UserName", ticket.AssignUserId);
            else
                sl = new SelectList(members, "Id", "UserName");

            var ticketData = new TicketData
            {
                Ticket = ticket,
                AssignUserName = assignUserName?.UserName,
                AuthorUserName = authorUserName?.UserName,
                Members = new SelectList(members, "Id", "UserName", ticket.AssignUserId),
                Comments = data
            };

            return View(ticketData);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteTicket(string id)
        {
            var ticket = await _context.Tickets.FindAsync(id);

            if (ticket == null)
            {
                ViewBag.ErrorMessage = $"Project with Id = {ticket.Id} cannot be found";
                return RedirectToAction("Index", "Home");
                //return View("NotFound");
            }

            var projectId = ticket.ProjectId;

            _context.Remove(ticket);
            await _context.SaveChangesAsync();

            return RedirectToAction("Project", "Projects", new { id = projectId });

        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Coder,User")]
        public async Task<IActionResult> AddComment(TicketData model)
        {
            var ticket = await _context.Tickets.FindAsync(model.Ticket.Id);

            if (ticket == null)
            {
                ViewBag.ErrorMessage = $"Project with Id = {ticket.Id} cannot be found";
                return RedirectToAction("Ticket", "Projects", new { id = ticket.Id });
                //return View("NotFound");
            }

            _context.TickertComments.Add(new TicketComment
            {
                TicketId = ticket.Id,
                AuthorId = model.NewCommentAuthorId,
                Comment = model.NewComment
            });

            await _context.SaveChangesAsync();

            return RedirectToAction("Ticket", "Projects", new { id = ticket.Id });
        }


        [HttpGet]
        async public Task<IActionResult> MyTickets()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tickets = _context.Tickets.Where(t => t.AssignUserId == userId).ToList();

            foreach(var ticket in tickets)
            {
                ticket.Project = await _context.Projects.FindAsync(ticket.ProjectId);
            }

            return View(tickets);
        }
    }
}