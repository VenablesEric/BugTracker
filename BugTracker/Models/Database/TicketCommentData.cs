using BugTracker.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models.Database
{
    public class TicketCommentData
    {
        public TicketComment TicketComment { get; set; }

        public string AuthorUserName { get; set; }
    }
}
