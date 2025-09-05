using System.ComponentModel.DataAnnotations;

namespace HippoCamp.MemoryStore.DTOs;

/// <summary>
/// Result data transfer object for batch operations.
/// Contains success/failure information and detailed results.
/// </summary>
public class BatchOperationResultDto
{
    /// <summary>
    /// Whether the entire batch operation was successful.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Total number of items processed in the batch.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Number of items that were successfully processed.
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Number of items that failed processing.
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// Collection of successfully created memory IDs (for create operations).
    /// </summary>
    public List<Guid> CreatedIds { get; set; } = new();

    /// <summary>
    /// Collection of errors that occurred during processing.
    /// </summary>
    public List<BatchItemError> Errors { get; set; } = new();

    /// <summary>
    /// Optional batch identifier that was provided with the request.
    /// </summary>
    public string? BatchId { get; set; }

    /// <summary>
    /// Timestamp when the batch operation was completed.
    /// </summary>
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Time taken to process the entire batch.
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }
}

/// <summary>
/// Represents an error that occurred for a specific item in a batch operation.
/// </summary>
public class BatchItemError
{
    /// <summary>
    /// Index of the item in the batch that caused the error (0-based).
    /// </summary>
    public int ItemIndex { get; set; }

    /// <summary>
    /// Error message describing what went wrong.
    /// </summary>
    [Required]
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Detailed validation errors for the item (if applicable).
    /// </summary>
    public Dictionary<string, string[]>? ValidationErrors { get; set; }

    /// <summary>
    /// The item data that caused the error (for debugging).
    /// </summary>
    public object? ItemData { get; set; }
}

/// <summary>
/// Progress information for long-running batch operations.
/// </summary>
public class BatchProgressInfo
{
    /// <summary>
    /// Current progress as a percentage (0-100).
    /// </summary>
    public int ProgressPercentage { get; set; }

    /// <summary>
    /// Number of items processed so far.
    /// </summary>
    public int ProcessedCount { get; set; }

    /// <summary>
    /// Total number of items to process.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Current status message.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Estimated time remaining for completion.
    /// </summary>
    public TimeSpan? EstimatedTimeRemaining { get; set; }
}