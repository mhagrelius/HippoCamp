using System.ComponentModel.DataAnnotations;
using HippoCamp.MemoryStore.Models;

namespace HippoCamp.MemoryStore.DTOs;

/// <summary>
/// Data transfer object for memory search operations.
/// Supports both text-based and vector-based search parameters.
/// </summary>
public class SearchMemoryDto
{
    /// <summary>
    /// Text query for content-based search.
    /// Optional - can be null if using vector search.
    /// </summary>
    [StringLength(1000, ErrorMessage = "Query cannot exceed 1,000 characters")]
    public string? Query { get; set; }

    /// <summary>
    /// Vector embedding for semantic similarity search.
    /// Optional - can be null if using text search.
    /// </summary>
    public float[]? QueryEmbedding { get; set; }

    /// <summary>
    /// Project identifier to filter results by.
    /// Optional - searches across all projects if not specified.
    /// </summary>
    [StringLength(200, ErrorMessage = "ProjectId cannot exceed 200 characters")]
    public string? ProjectId { get; set; }

    /// <summary>
    /// Memory types to include in search results.
    /// Optional - includes all types if not specified.
    /// </summary>
    public List<MemoryType>? Types { get; set; }

    /// <summary>
    /// Whether to include deprecated memories in results.
    /// Defaults to false.
    /// </summary>
    public bool IncludeDeprecated { get; set; } = false;

    /// <summary>
    /// Maximum number of results to return.
    /// Must be between 1 and 100.
    /// </summary>
    [Range(1, 100, ErrorMessage = "Limit must be between 1 and 100")]
    public int Limit { get; set; } = 10;

    /// <summary>
    /// Minimum relevance score threshold for results.
    /// Must be between 0.0 and 1.0.
    /// </summary>
    [Range(0.0, 1.0, ErrorMessage = "MinimumScore must be between 0.0 and 1.0")]
    public double MinimumScore { get; set; } = 0.0;

    /// <summary>
    /// Date range filter - start date.
    /// Optional - no start date filter if not specified.
    /// </summary>
    public DateTime? CreatedAfter { get; set; }

    /// <summary>
    /// Date range filter - end date.
    /// Optional - no end date filter if not specified.
    /// </summary>
    public DateTime? CreatedBefore { get; set; }

    /// <summary>
    /// Metadata filter criteria.
    /// Key-value pairs that must match in the memory's metadata.
    /// Optional - no metadata filtering if not specified.
    /// </summary>
    public Dictionary<string, object>? MetadataFilter { get; set; }

    /// <summary>
    /// Validates that either Query or QueryEmbedding is provided.
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Query) || (QueryEmbedding != null && QueryEmbedding.Length > 0);
    }
}