using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Pgvector;

namespace HippoCamp.MemoryStore.Models;

/// <summary>
/// Core entity representing a memory item in the memory store.
/// Contains content, embeddings, and metadata for semantic operations.
/// </summary>
[Index(nameof(ProjectId))]
[Index(nameof(Type))]
[Index(nameof(Created))]
[Index(nameof(LastAccessed))]
[Index(nameof(IsDeprecated))]
[Index(nameof(IsDeleted))]
public class Memory
{
    /// <summary>
    /// Unique identifier for the memory.
    /// </summary>
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Project identifier this memory belongs to.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string ProjectId { get; set; } = string.Empty;

    /// <summary>
    /// The actual content/text of the memory.
    /// </summary>
    [Required]
    [StringLength(10000)]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Vector embedding for semantic search operations.
    /// </summary>
    public Vector? Embedding { get; set; }

    /// <summary>
    /// Type categorization of this memory.
    /// </summary>
    [Required]
    public MemoryType Type { get; set; }

    /// <summary>
    /// Timestamp when the memory was created.
    /// </summary>
    public DateTime Created { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when the memory was last accessed.
    /// </summary>
    public DateTime LastAccessed { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Additional metadata stored as JSON.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Indicates if this memory is deprecated and should not be used.
    /// </summary>
    public bool IsDeprecated { get; set; } = false;

    /// <summary>
    /// Soft delete flag - memory is logically deleted but retained for retention period.
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Timestamp when the memory was soft deleted.
    /// Null if memory is not deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}