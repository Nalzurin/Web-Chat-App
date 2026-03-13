using System.ComponentModel.DataAnnotations;

namespace back_end.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public required string Username { get; set; }

        [EmailAddress]
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
