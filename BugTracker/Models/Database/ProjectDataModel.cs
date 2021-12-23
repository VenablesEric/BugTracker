using BugTracker.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BugTracker.Models.Database
{
    public class ProjectDataModel
    {
        public Project Infomation { get; set; }
        public List<BugTrackerUser> Members { get; set; }
        public List<SelectListItem> NonMembers { get; set;}
        public List<string> SelectedNonMembers { get; set; }
        public List<Ticket> Tickets { get; set; }
        public Ticket NewTick { get; set; }
    }
}
