using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
