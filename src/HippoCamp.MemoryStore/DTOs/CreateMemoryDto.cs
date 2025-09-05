using System.ComponentModel.DataAnnotations;
using HippoCamp.MemoryStore.Models;

namespace HippoCamp.MemoryStore.DTOs;

/// <summary>
/// Data transfer object for creating a new memory.
/// </summary>
public class CreateMemoryDto
{
    /// <summary>
    /// Project identifier this memory belongs to.
    /// </summary>
    [Required(ErrorMessage = "ProjectId is required")]
    [StringLength(200, ErrorMessage = "ProjectId cannot exceed 200 characters")]
    public string ProjectId { get; set; } = string.Empty;

    /// <summary>
    /// The actual content/text of the memory.
    /// </summary>
    [Required(ErrorMessage = "Content is required")]
    [StringLength(10000, ErrorMessage = "Content cannot exceed 10,000 characters")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Vector embedding for semantic search operations.
    /// Optional - can be generated on the server side.
    /// </summary>
    public float[]? Embedding { get; set; }

    /// <summary>
    /// Type categorization of this memory.
    /// </summary>
    [Required(ErrorMessage = "Type is required")]
    public MemoryType Type { get; set; }

    /// <summary>
    /// Additional metadata stored as JSON.
    /// Optional - defaults to empty dictionary if not provided.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}