using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;
using HippoCamp.MemoryStore.Models;

namespace HippoCamp.MemoryStore;

public class MemoryDbContext : DbContext
{
    public MemoryDbContext(DbContextOptions<MemoryDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Memory entities in the database.
    /// </summary>
    public DbSet<Memory> Memories { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Enable the pgvector extension
        modelBuilder.HasPostgresExtension("vector");
    }
}