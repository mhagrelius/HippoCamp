using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HippoCamp.MemoryStore.DTOs;
using HippoCamp.MemoryStore.Models;
using System.ComponentModel.DataAnnotations;
using Pgvector;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace HippoCamp.MemoryStore.Services;

/// <summary>
/// Service for batch memory operations providing efficient bulk processing
/// with transaction support and progress reporting.
/// </summary>
public class BatchMemoryService : IBatchMemoryService
{
    private readonly MemoryDbContext _context;
    private readonly ILogger<BatchMemoryService> _logger;
    private readonly ConcurrentDictionary<string, BatchProgressInfo> _batchProgress = new();

    public BatchMemoryService(MemoryDbContext context, ILogger<BatchMemoryService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<BatchOperationResultDto> CreateBatchAsync(
        BatchCreateMemoryDto batchRequest,
        IProgress<BatchProgressInfo>? progressCallback = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(batchRequest);

        var stopwatch = Stopwatch.StartNew();
        var batchId = batchRequest.BatchId ?? Guid.NewGuid().ToString();
        var result = new BatchOperationResultDto
        {
            BatchId = batchId,
            TotalCount = batchRequest.Memories.Count
        };

        _logger.LogInformation("Starting batch create operation for {Count} memories with batch ID {BatchId}",
            batchRequest.Memories.Count, batchId);

        try
        {
            // Validate the batch first
            var validationResult = await ValidateBatchAsync(batchRequest, cancellationToken);
            if (!validationResult.IsValid)
            {
                result.IsSuccess = false;
                result.Errors = validationResult.ValidationErrors;
                result.FailureCount = validationResult.InvalidCount;
                result.ProcessingTime = stopwatch.Elapsed;
                return result;
            }

            // Initialize progress tracking
            var progressInfo = new BatchProgressInfo
            {
                TotalCount = batchRequest.Memories.Count,
                Status = "Starting batch creation"
            };

            if (batchRequest.ReportProgress)
            {
                _batchProgress[batchId] = progressInfo;
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var createdMemories = new List<Memory>();
                var errors = new List<BatchItemError>();

                for (int i = 0; i < batchRequest.Memories.Count; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var createDto = batchRequest.Memories[i];

                    try
                    {
                        var memory = new Memory
                        {
                            ProjectId = createDto.ProjectId,
                            Content = createDto.Content,
                            Type = createDto.Type,
                            Metadata = createDto.Metadata,
                            Embedding = createDto.Embedding?.ToArray() is { } embArray 
                                ? new Vector(embArray) 
                                : null
                        };

                        _context.Memories.Add(memory);
                        createdMemories.Add(memory);
                        result.SuccessCount++;

                        // Update progress
                        if (batchRequest.ReportProgress)
                        {
                            progressInfo.ProcessedCount = i + 1;
                            progressInfo.ProgressPercentage = (int)((double)(i + 1) / batchRequest.Memories.Count * 100);
                            progressInfo.Status = $"Created memory {i + 1} of {batchRequest.Memories.Count}";
                            progressCallback?.Report(progressInfo);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to create memory at index {Index}", i);

                        var error = new BatchItemError
                        {
                            ItemIndex = i,
                            ErrorMessage = ex.Message,
                            ItemData = createDto
                        };

                        errors.Add(error);
                        result.FailureCount++;

                        if (!batchRequest.ContinueOnError)
                        {
                            throw;
                        }
                    }
                }

                // Save all changes within the transaction
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                result.CreatedIds = createdMemories.Select(m => m.Id).ToList();
                result.IsSuccess = result.FailureCount == 0 || batchRequest.ContinueOnError;
                result.Errors = errors;

                _logger.LogInformation("Batch create completed. Success: {Success}, Failures: {Failures}",
                    result.SuccessCount, result.FailureCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during batch create operation, rolling back transaction");
                await transaction.RollbackAsync(cancellationToken);
                result.IsSuccess = false;
                result.FailureCount = result.TotalCount;
                result.SuccessCount = 0;

                if (!result.Errors.Any())
                {
                    result.Errors.Add(new BatchItemError
                    {
                        ItemIndex = -1,
                        ErrorMessage = $"Batch operation failed: {ex.Message}"
                    });
                }

                throw;
            }
        }
        catch (Exception ex) when (!(ex is OperationCanceledException))
        {
            _logger.LogError(ex, "Unexpected error during batch create operation");
            result.IsSuccess = false;
        }
        finally
        {
            result.ProcessingTime = stopwatch.Elapsed;
            
            // Clean up progress tracking
            if (batchRequest.ReportProgress && _batchProgress.ContainsKey(batchId))
            {
                _batchProgress.TryRemove(batchId, out _);
            }
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<BatchOperationResultDto> UpdateBatchAsync(
        Dictionary<Guid, UpdateMemoryDto> updates,
        IProgress<BatchProgressInfo>? progressCallback = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(updates);

        var stopwatch = Stopwatch.StartNew();
        var batchId = Guid.NewGuid().ToString();
        var result = new BatchOperationResultDto
        {
            BatchId = batchId,
            TotalCount = updates.Count
        };

        if (updates.Count > 100)
        {
            throw new ArgumentException("Maximum 100 updates allowed per batch", nameof(updates));
        }

        _logger.LogInformation("Starting batch update operation for {Count} memories", updates.Count);

        try
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var errors = new List<BatchItemError>();
                var memoryIds = updates.Keys.ToList();
                var existingMemories = await _context.Memories
                    .Where(m => memoryIds.Contains(m.Id))
                    .ToListAsync(cancellationToken);

                var itemIndex = 0;
                foreach (var (memoryId, updateDto) in updates)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var memory = existingMemories.FirstOrDefault(m => m.Id == memoryId);
                    if (memory == null)
                    {
                        errors.Add(new BatchItemError
                        {
                            ItemIndex = itemIndex,
                            ErrorMessage = $"Memory with ID {memoryId} not found"
                        });
                        result.FailureCount++;
                        itemIndex++;
                        continue;
                    }

                    try
                    {
                        // Update memory properties
                        if (!string.IsNullOrEmpty(updateDto.Content))
                            memory.Content = updateDto.Content;
                        
                        if (updateDto.Embedding != null)
                            memory.Embedding = new Vector(updateDto.Embedding);
                        
                        if (updateDto.Metadata != null)
                            memory.Metadata = updateDto.Metadata;
                        
                        if (updateDto.IsDeprecated.HasValue)
                            memory.IsDeprecated = updateDto.IsDeprecated.Value;

                        memory.LastAccessed = DateTime.UtcNow;

                        result.SuccessCount++;

                        // Report progress
                        progressCallback?.Report(new BatchProgressInfo
                        {
                            ProcessedCount = itemIndex + 1,
                            TotalCount = updates.Count,
                            ProgressPercentage = (int)((double)(itemIndex + 1) / updates.Count * 100),
                            Status = $"Updated memory {itemIndex + 1} of {updates.Count}"
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to update memory {MemoryId}", memoryId);
                        errors.Add(new BatchItemError
                        {
                            ItemIndex = itemIndex,
                            ErrorMessage = ex.Message,
                            ItemData = updateDto
                        });
                        result.FailureCount++;
                    }

                    itemIndex++;
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                result.IsSuccess = result.FailureCount == 0;
                result.Errors = errors;

                _logger.LogInformation("Batch update completed. Success: {Success}, Failures: {Failures}",
                    result.SuccessCount, result.FailureCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during batch update operation, rolling back transaction");
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
        catch (Exception ex) when (!(ex is OperationCanceledException))
        {
            _logger.LogError(ex, "Unexpected error during batch update operation");
            result.IsSuccess = false;
        }
        finally
        {
            result.ProcessingTime = stopwatch.Elapsed;
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<BatchOperationResultDto> BulkDeprecateAsync(
        BulkDeprecationCriteria criteria,
        IProgress<BatchProgressInfo>? progressCallback = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(criteria);

        var stopwatch = Stopwatch.StartNew();
        var batchId = Guid.NewGuid().ToString();
        var result = new BatchOperationResultDto { BatchId = batchId };

        _logger.LogInformation("Starting bulk deprecation with criteria: ProjectId={ProjectId}, Type={Type}, MaxCount={MaxCount}",
            criteria.ProjectId, criteria.Type, criteria.MaxCount);

        try
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // Build query based on criteria
                var query = _context.Memories.AsQueryable();

                if (!string.IsNullOrEmpty(criteria.ProjectId))
                    query = query.Where(m => m.ProjectId == criteria.ProjectId);

                if (criteria.Type.HasValue)
                    query = query.Where(m => m.Type == criteria.Type.Value);

                if (criteria.CreatedBefore.HasValue)
                    query = query.Where(m => m.Created < criteria.CreatedBefore.Value);

                if (criteria.LastAccessedBefore.HasValue)
                    query = query.Where(m => m.LastAccessed < criteria.LastAccessedBefore.Value);

                if (!criteria.IncludeAlreadyDeprecated)
                    query = query.Where(m => !m.IsDeprecated);

                // Apply metadata filters if specified
                if (criteria.MetadataFilters?.Any() == true)
                {
                    foreach (var filter in criteria.MetadataFilters)
                    {
                        query = query.Where(m => m.Metadata.ContainsKey(filter.Key) && 
                                               m.Metadata[filter.Key].Equals(filter.Value));
                    }
                }

                // Get memories to deprecate with limit
                var memoriesToDeprecate = await query
                    .Take(criteria.MaxCount)
                    .ToListAsync(cancellationToken);

                result.TotalCount = memoriesToDeprecate.Count;

                if (memoriesToDeprecate.Count == 0)
                {
                    result.IsSuccess = true;
                    _logger.LogInformation("No memories found matching deprecation criteria");
                    return result;
                }

                // Deprecate memories in batches
                for (int i = 0; i < memoriesToDeprecate.Count; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var memory = memoriesToDeprecate[i];
                    memory.IsDeprecated = true;
                    result.SuccessCount++;

                    // Report progress every 10 items or on last item
                    if ((i + 1) % 10 == 0 || i == memoriesToDeprecate.Count - 1)
                    {
                        progressCallback?.Report(new BatchProgressInfo
                        {
                            ProcessedCount = i + 1,
                            TotalCount = memoriesToDeprecate.Count,
                            ProgressPercentage = (int)((double)(i + 1) / memoriesToDeprecate.Count * 100),
                            Status = $"Deprecated {i + 1} of {memoriesToDeprecate.Count} memories"
                        });
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                result.IsSuccess = true;

                _logger.LogInformation("Bulk deprecation completed. Deprecated {Count} memories", result.SuccessCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk deprecation, rolling back transaction");
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
        catch (Exception ex) when (!(ex is OperationCanceledException))
        {
            _logger.LogError(ex, "Unexpected error during bulk deprecation");
            result.IsSuccess = false;
            result.Errors.Add(new BatchItemError
            {
                ItemIndex = -1,
                ErrorMessage = ex.Message
            });
        }
        finally
        {
            result.ProcessingTime = stopwatch.Elapsed;
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<BatchValidationResult> ValidateBatchAsync(
        BatchCreateMemoryDto batchRequest,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(batchRequest);

        var result = new BatchValidationResult
        {
            TotalCount = batchRequest.Memories.Count
        };

        // Validate batch size constraint
        if (batchRequest.Memories.Count > 100)
        {
            result.ValidationErrors.Add(new BatchItemError
            {
                ItemIndex = -1,
                ErrorMessage = "Maximum 100 memories allowed per batch operation"
            });
            return result;
        }

        if (batchRequest.Memories.Count == 0)
        {
            result.ValidationErrors.Add(new BatchItemError
            {
                ItemIndex = -1,
                ErrorMessage = "At least one memory is required"
            });
            return result;
        }

        // Validate each memory in the batch
        for (int i = 0; i < batchRequest.Memories.Count; i++)
        {
            var memory = batchRequest.Memories[i];
            var validationErrors = new List<ValidationResult>();
            var validationContext = new ValidationContext(memory);

            if (!Validator.TryValidateObject(memory, validationContext, validationErrors, true))
            {
                result.InvalidCount++;
                result.ValidationErrors.Add(new BatchItemError
                {
                    ItemIndex = i,
                    ErrorMessage = "Validation failed",
                    ValidationErrors = validationErrors
                        .GroupBy(e => e.MemberNames.FirstOrDefault() ?? "General")
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage ?? "Validation error").ToArray()
                        ),
                    ItemData = memory
                });
            }
            else
            {
                result.ValidCount++;
            }

            // Additional business logic validation
            if (memory.Embedding?.Length > 0 && memory.Embedding.Length != 1536)
            {
                result.Warnings.Add($"Memory at index {i}: Embedding dimension {memory.Embedding.Length} may not be compatible with standard models (expected 1536)");
            }

            if (memory.Content.Length > 8000)
            {
                result.Warnings.Add($"Memory at index {i}: Content length {memory.Content.Length} is approaching the limit");
            }
        }

        // Check for duplicate ProjectId + Content combinations within the batch
        var duplicates = batchRequest.Memories
            .Select((memory, index) => new { memory.ProjectId, memory.Content, Index = index })
            .GroupBy(x => new { x.ProjectId, x.Content })
            .Where(g => g.Count() > 1)
            .ToList();

        foreach (var duplicateGroup in duplicates)
        {
            var indices = duplicateGroup.Select(x => x.Index).ToList();
            result.Warnings.Add($"Duplicate content found at indices: {string.Join(", ", indices)}");
        }

        result.IsValid = result.InvalidCount == 0;

        return result;
    }

    /// <inheritdoc />
    public async Task<BatchProgressInfo?> GetBatchStatusAsync(
        string batchId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(batchId);

        return await Task.FromResult(_batchProgress.GetValueOrDefault(batchId));
    }
}