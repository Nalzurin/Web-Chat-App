using back_end.Models;
using Microsoft.EntityFrameworkCore;

namespace back_end.Data;
public class ChatDbContext : DbContext
{
    public ChatDbContext(DbContextOptions<ChatDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
}
