using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace back_end.Models
{
    // Domain User derives from IdentityUser so it can be used directly with ASP.NET Core Identity
    public class User : IdentityUser<Guid>
    {
        // Keep a domain-friendly Username property that delegates to Identity's UserName
        public string Username
        {
            get
            {
                return base.UserName ?? string.Empty;
            }
            set
            {
                base.UserName = value;
            }
        }

        // Expose Email with validation attribute while delegating to Identity's Email
        [EmailAddress]
        public new string Email
        {
            get
            {
                return base.Email ?? string.Empty;
            }
            set
            {
                base.Email = value;
            }
        }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
