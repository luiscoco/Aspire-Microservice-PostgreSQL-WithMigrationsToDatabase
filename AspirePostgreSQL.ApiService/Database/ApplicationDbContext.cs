using Microsoft.EntityFrameworkCore;
using AspirePostgreSQL.Entities;

namespace AspirePostgreSQL.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("content");
    }

    public DbSet<Article> Articles { get; set; }
}
