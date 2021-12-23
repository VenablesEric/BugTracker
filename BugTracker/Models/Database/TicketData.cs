using BugTracker.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BugTracker.Models.Database
{
    public class TicketData
    {
        public Ticket Ticket { get; set; }
        public string AuthorUserName { get; set; }
        public string AssignUserName { get; set; }
        public SelectList Members { get; set; }
        public List<TicketCommentData> Comments { get; set; }
        public string NewCommentAuthorId { get; set; }
        public string NewComment { get; set; }
    }
}
