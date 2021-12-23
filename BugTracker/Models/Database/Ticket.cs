using BugTracker.Areas.Identity.Data;
using BugTracker.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models
{
    public class Ticket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public Project Project { get; set; }

        public string AuthorUserId { get; set; }
        //[ForeignKey("AuthorUserId")]
        //public BugTrackerUser AuthorUser { get; set; }

        public string AssignUserId { get; set; }
        //[ForeignKey("AssignUserId")]
       // public BugTrackerUser AssignUser { get; set; }

        [Required]
        [MinLength(1)]
        [StringLength(128)]
        public string Title { get; set; }

        [StringLength(1024)]
        public string Description { get; set; }

        public TICKET_TYPE Type { get; set; }
        public TICKET_PRIORITY Priority { get; set; }
        public TICKET_STATUS Status { get; set; }

    }
}
