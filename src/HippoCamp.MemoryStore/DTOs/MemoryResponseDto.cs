using HippoCamp.MemoryStore.Models;

namespace HippoCamp.MemoryStore.DTOs;

/// <summary>
/// Data transfer object for memory API responses.
/// Contains all memory information for client consumption.
/// </summary>
public class MemoryResponseDto
{
    /// <summary>
    /// Unique identifier for the memory.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Project identifier this memory belongs to.
    /// </summary>
    public string ProjectId { get; set; } = string.Empty;

    /// <summary>
    /// The actual content/text of the memory.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Vector embedding for semantic search operations.
    /// Exposed as float array for client consumption.
    /// </summary>
    public float[]? Embedding { get; set; }

    /// <summary>
    /// Type categorization of this memory.
    /// </summary>
    public MemoryType Type { get; set; }

    /// <summary>
    /// Timestamp when the memory was created.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Timestamp when the memory was last accessed.
    /// </summary>
    public DateTime LastAccessed { get; set; }

    /// <summary>
    /// Additional metadata stored as JSON.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Indicates if this memory is deprecated and should not be used.
    /// </summary>
    public bool IsDeprecated { get; set; }

    /// <summary>
    /// Relevance score when returned from search operations.
    /// Only populated during search results.
    /// </summary>
    public double? RelevanceScore { get; set; }
}