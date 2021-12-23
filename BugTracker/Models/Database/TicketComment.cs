using BugTracker.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models
{
    public class TicketComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string TicketId { get; set; }
        [ForeignKey("TicketId")]
        public Ticket Ticket { get; set; }

        public string AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public BugTrackerUser Author { get; set; }

        [MinLength(1)]
        [Required]
        [StringLength(256)]
        public string Comment { get; set; }

    }
}
