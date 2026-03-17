using back_end.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace back_end.Data;
public class ChatDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public ChatDbContext(DbContextOptions<ChatDbContext> options)
        : base(options)
    {
    }

    public DbSet<back_end.Models.KeyBundle>? KeyBundles { get; set; }
    public DbSet<back_end.Models.OneTimePreKey>? OneTimePreKeys { get; set; }
}
