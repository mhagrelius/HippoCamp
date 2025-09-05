using HippoCamp.MemoryStore.DTOs;
using HippoCamp.MemoryStore.Models;

namespace HippoCamp.MemoryStore.Services;

/// <summary>
/// Interface for batch memory operations providing efficient bulk processing
/// with transaction support and progress reporting.
/// </summary>
public interface IBatchMemoryService
{
    /// <summary>
    /// Creates multiple memories in a single batch operation.
    /// Supports up to 100 memories per batch with transaction rollback on failure.
    /// </summary>
    /// <param name="batchRequest">Batch creation request containing memories to create</param>
    /// <param name="progressCallback">Optional callback for progress reporting</param>
    /// <param name="cancellationToken">Cancellation token for operation cancellation</param>
    /// <returns>Result containing success/failure information and created memory IDs</returns>
    Task<BatchOperationResultDto> CreateBatchAsync(
        BatchCreateMemoryDto batchRequest,
        IProgress<BatchProgressInfo>? progressCallback = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates multiple memories in a single batch operation with transaction support.
    /// </summary>
    /// <param name="updates">Dictionary of memory IDs to their update DTOs</param>
    /// <param name="progressCallback">Optional callback for progress reporting</param>
    /// <param name="cancellationToken">Cancellation token for operation cancellation</param>
    /// <returns>Result containing success/failure information and updated memory count</returns>
    Task<BatchOperationResultDto> UpdateBatchAsync(
        Dictionary<Guid, UpdateMemoryDto> updates,
        IProgress<BatchProgressInfo>? progressCallback = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Bulk deprecation of memories based on specified criteria.
    /// </summary>
    /// <param name="criteria">Criteria for selecting memories to deprecate</param>
    /// <param name="progressCallback">Optional callback for progress reporting</param>
    /// <param name="cancellationToken">Cancellation token for operation cancellation</param>
    /// <returns>Result containing count of deprecated memories</returns>
    Task<BatchOperationResultDto> BulkDeprecateAsync(
        BulkDeprecationCriteria criteria,
        IProgress<BatchProgressInfo>? progressCallback = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a batch request without executing it.
    /// Useful for pre-validation before committing to batch operations.
    /// </summary>
    /// <param name="batchRequest">Batch request to validate</param>
    /// <param name="cancellationToken">Cancellation token for operation cancellation</param>
    /// <returns>Validation result with detailed error information</returns>
    Task<BatchValidationResult> ValidateBatchAsync(
        BatchCreateMemoryDto batchRequest,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current status of a long-running batch operation.
    /// </summary>
    /// <param name="batchId">Identifier of the batch operation</param>
    /// <param name="cancellationToken">Cancellation token for operation cancellation</param>
    /// <returns>Current progress information or null if batch not found</returns>
    Task<BatchProgressInfo?> GetBatchStatusAsync(
        string batchId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Criteria for bulk deprecation operations.
/// </summary>
public class BulkDeprecationCriteria
{
    /// <summary>
    /// Optional project ID to limit deprecation to specific project.
    /// </summary>
    public string? ProjectId { get; set; }

    /// <summary>
    /// Optional memory type to limit deprecation to specific type.
    /// </summary>
    public MemoryType? Type { get; set; }

    /// <summary>
    /// Deprecate memories older than this date.
    /// </summary>
    public DateTime? CreatedBefore { get; set; }

    /// <summary>
    /// Deprecate memories not accessed since this date.
    /// </summary>
    public DateTime? LastAccessedBefore { get; set; }

    /// <summary>
    /// Optional metadata filters for more specific criteria.
    /// </summary>
    public Dictionary<string, object>? MetadataFilters { get; set; }

    /// <summary>
    /// Maximum number of memories to deprecate in single operation.
    /// Defaults to 1000 to prevent accidentally deprecating entire database.
    /// </summary>
    public int MaxCount { get; set; } = 1000;

    /// <summary>
    /// Whether to include already deprecated memories in the count.
    /// </summary>
    public bool IncludeAlreadyDeprecated { get; set; } = false;
}

/// <summary>
/// Result of batch validation operations.
/// </summary>
public class BatchValidationResult
{
    /// <summary>
    /// Whether the entire batch passed validation.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Total number of items in the batch.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Number of items that passed validation.
    /// </summary>
    public int ValidCount { get; set; }

    /// <summary>
    /// Number of items that failed validation.
    /// </summary>
    public int InvalidCount { get; set; }

    /// <summary>
    /// Collection of validation errors for invalid items.
    /// </summary>
    public List<BatchItemError> ValidationErrors { get; set; } = new();

    /// <summary>
    /// Warnings that don't prevent batch execution but should be noted.
    /// </summary>
    public List<string> Warnings { get; set; } = new();
}