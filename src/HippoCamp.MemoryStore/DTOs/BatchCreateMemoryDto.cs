using System.ComponentModel.DataAnnotations;

namespace HippoCamp.MemoryStore.DTOs;

/// <summary>
/// Data transfer object for batch memory creation operations.
/// Contains a collection of memory creation requests with batch constraints.
/// </summary>
public class BatchCreateMemoryDto
{
    /// <summary>
    /// Collection of memory creation requests.
    /// Maximum 100 memories per batch operation.
    /// </summary>
    [Required(ErrorMessage = "Memories collection is required")]
    [MinLength(1, ErrorMessage = "At least one memory is required")]
    [MaxLength(100, ErrorMessage = "Maximum 100 memories allowed per batch")]
    public List<CreateMemoryDto> Memories { get; set; } = new();

    /// <summary>
    /// Optional batch identifier for tracking purposes.
    /// </summary>
    public string? BatchId { get; set; }

    /// <summary>
    /// Whether to continue processing if some memories fail validation.
    /// Default is false - any failure rolls back the entire batch.
    /// </summary>
    public bool ContinueOnError { get; set; } = false;

    /// <summary>
    /// Whether to report progress during batch processing.
    /// Useful for large batches to track completion status.
    /// </summary>
    public bool ReportProgress { get; set; } = false;
}