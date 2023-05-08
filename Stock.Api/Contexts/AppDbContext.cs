using Microsoft.EntityFrameworkCore;

namespace Stock.Api.Contexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {
    }

    public DbSet<Models.Stock> Stocks { get; set; }
}