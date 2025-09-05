using System.ComponentModel.DataAnnotations;
using HippoCamp.MemoryStore.Models;

namespace HippoCamp.MemoryStore.DTOs;

/// <summary>
/// Data transfer object for updating an existing memory.
/// All fields are optional to support partial updates.
/// </summary>
public class UpdateMemoryDto
{
    /// <summary>
    /// Updated content/text of the memory.
    /// Optional - existing content is preserved if not provided.
    /// </summary>
    [StringLength(10000, ErrorMessage = "Content cannot exceed 10,000 characters")]
    public string? Content { get; set; }

    /// <summary>
    /// Updated vector embedding for semantic search operations.
    /// Optional - existing embedding is preserved if not provided.
    /// </summary>
    public float[]? Embedding { get; set; }

    /// <summary>
    /// Updated type categorization of this memory.
    /// Optional - existing type is preserved if not provided.
    /// </summary>
    public MemoryType? Type { get; set; }

    /// <summary>
    /// Updated metadata stored as JSON.
    /// Optional - existing metadata is preserved if not provided.
    /// Note: This completely replaces existing metadata, does not merge.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>
    /// Updated deprecation status.
    /// Optional - existing status is preserved if not provided.
    /// </summary>
    public bool? IsDeprecated { get; set; }

    /// <summary>
    /// Whether to update the LastAccessed timestamp.
    /// Defaults to true - accessing a memory for update should update the timestamp.
    /// </summary>
    public bool UpdateLastAccessed { get; set; } = true;

    /// <summary>
    /// Validates that at least one field is being updated.
    /// </summary>
    public bool HasUpdates()
    {
        return !string.IsNullOrEmpty(Content) ||
               Embedding != null ||
               Type.HasValue ||
               Metadata != null ||
               IsDeprecated.HasValue;
    }
}