using Microsoft.EntityFrameworkCore;
using Order.Api.Models;

namespace Order.Api.Contexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> contextOptions): base(contextOptions)
    {
        
    }

    public DbSet<Models.Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
}