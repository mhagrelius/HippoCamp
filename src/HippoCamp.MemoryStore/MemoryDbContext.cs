using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;

namespace HippoCamp.MemoryStore;

public class MemoryDbContext : DbContext
{
    public MemoryDbContext(DbContextOptions<MemoryDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Enable the pgvector extension
        modelBuilder.HasPostgresExtension("vector");
    }
}