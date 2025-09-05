using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HippoCamp.MemoryStore.DTOs;
using HippoCamp.MemoryStore.Models;
using Pgvector;

namespace HippoCamp.MemoryStore.Services;

/// <summary>
/// Service implementation for memory CRUD operations.
/// Handles all memory operations including creation, retrieval, updates, and soft deletion.
/// </summary>
public class MemoryService : IMemoryService
{
    private readonly MemoryDbContext _context;
    private readonly ILogger<MemoryService> _logger;
    private const int MaxEmbeddingDimensions = 4096; // Configurable max dimensions
    private const int MinEmbeddingDimensions = 50;   // Configurable min dimensions

    public MemoryService(MemoryDbContext context, ILogger<MemoryService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates a new memory with embeddings and metadata.
    /// </summary>
    public async Task<MemoryResponseDto> CreateAsync(CreateMemoryDto createDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(createDto);

        _logger.LogInformation("Creating new memory for project {ProjectId}", createDto.ProjectId);

        // Validate business rules
        ValidateCreateRequest(createDto);

        // Create memory entity
        var memory = new Memory
        {
            Id = Guid.NewGuid(),
            ProjectId = createDto.ProjectId,
            Content = createDto.Content,
            Embedding = createDto.Embedding != null ? new Vector(createDto.Embedding) : null,
            Type = createDto.Type,
            Metadata = createDto.Metadata,
            Created = DateTime.UtcNow,
            LastAccessed = DateTime.UtcNow,
            IsDeprecated = false,
            IsDeleted = false
        };

        // Add to database
        _context.Memories.Add(memory);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully created memory {MemoryId} for project {ProjectId}", 
            memory.Id, memory.ProjectId);

        return MapToResponseDto(memory);
    }

    /// <summary>
    /// Retrieves a memory by its unique identifier.
    /// </summary>
    public async Task<MemoryResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving memory by ID {MemoryId}", id);

        var memory = await _context.Memories
            .Where(m => m.Id == id && !m.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (memory == null)
        {
            _logger.LogInformation("Memory {MemoryId} not found or is deleted", id);
            return null;
        }

        // Update last accessed timestamp
        memory.LastAccessed = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogDebug("Successfully retrieved memory {MemoryId}", id);
        return MapToResponseDto(memory);
    }

    /// <summary>
    /// Retrieves all non-deleted memories for a specific project.
    /// </summary>
    public async Task<List<MemoryResponseDto>> GetByProjectAsync(string projectId, bool includeDeprecated = false, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectId);

        _logger.LogInformation("Retrieving memories for project {ProjectId}, includeDeprecated: {IncludeDeprecated}", 
            projectId, includeDeprecated);

        var query = _context.Memories
            .Where(m => m.ProjectId == projectId && !m.IsDeleted);

        if (!includeDeprecated)
        {
            query = query.Where(m => !m.IsDeprecated);
        }

        var memories = await query
            .OrderByDescending(m => m.LastAccessed)
            .ThenByDescending(m => m.Created)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {MemoryCount} memories for project {ProjectId}", 
            memories.Count, projectId);

        return memories.Select(MapToResponseDto).ToList();
    }

    /// <summary>
    /// Updates an existing memory with new content, metadata, or deprecation status.
    /// </summary>
    public async Task<MemoryResponseDto?> UpdateAsync(Guid id, UpdateMemoryDto updateDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(updateDto);

        if (!updateDto.HasUpdates())
        {
            _logger.LogWarning("Update request for memory {MemoryId} contains no changes", id);
            throw new ArgumentException("At least one field must be updated", nameof(updateDto));
        }

        _logger.LogInformation("Updating memory {MemoryId}", id);

        var memory = await _context.Memories
            .Where(m => m.Id == id && !m.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (memory == null)
        {
            _logger.LogInformation("Memory {MemoryId} not found or is deleted", id);
            return null;
        }

        // Validate update request
        ValidateUpdateRequest(updateDto);

        // Apply updates
        if (!string.IsNullOrEmpty(updateDto.Content))
        {
            memory.Content = updateDto.Content;
        }

        if (updateDto.Embedding != null)
        {
            memory.Embedding = new Vector(updateDto.Embedding);
        }

        if (updateDto.Type.HasValue)
        {
            memory.Type = updateDto.Type.Value;
        }

        if (updateDto.Metadata != null)
        {
            memory.Metadata = updateDto.Metadata;
        }

        if (updateDto.IsDeprecated.HasValue)
        {
            memory.IsDeprecated = updateDto.IsDeprecated.Value;
        }

        if (updateDto.UpdateLastAccessed)
        {
            memory.LastAccessed = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully updated memory {MemoryId}", id);
        return MapToResponseDto(memory);
    }

    /// <summary>
    /// Performs soft delete on a memory by setting IsDeleted flag.
    /// </summary>
    public async Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Soft deleting memory {MemoryId}", id);

        var memory = await _context.Memories
            .Where(m => m.Id == id && !m.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (memory == null)
        {
            _logger.LogInformation("Memory {MemoryId} not found or already deleted", id);
            return false;
        }

        memory.IsDeleted = true;
        memory.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully soft deleted memory {MemoryId}", id);
        return true;
    }

    /// <summary>
    /// Validates if an embedding array has consistent dimensions and valid values.
    /// </summary>
    public bool ValidateEmbedding(float[]? embedding)
    {
        if (embedding == null)
        {
            return false; // Embeddings are required
        }

        // Check dimensions
        if (embedding.Length < MinEmbeddingDimensions || embedding.Length > MaxEmbeddingDimensions)
        {
            _logger.LogDebug("Embedding dimension validation failed: {Dimensions} (expected between {Min} and {Max})", 
                embedding.Length, MinEmbeddingDimensions, MaxEmbeddingDimensions);
            return false;
        }

        // Check for invalid values (NaN, Infinity)
        for (int i = 0; i < embedding.Length; i++)
        {
            if (float.IsNaN(embedding[i]) || float.IsInfinity(embedding[i]))
            {
                _logger.LogDebug("Embedding contains invalid value at index {Index}: {Value}", i, embedding[i]);
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Validates create memory request for business rules.
    /// </summary>
    private void ValidateCreateRequest(CreateMemoryDto createDto)
    {
        var errors = new Dictionary<string, List<string>>();

        // Validate ProjectId
        if (string.IsNullOrWhiteSpace(createDto.ProjectId))
        {
            errors.GetOrAdd("ProjectId").Add("Project ID is required");
        }
        else if (createDto.ProjectId.Length > 200)
        {
            errors.GetOrAdd("ProjectId").Add("Project ID cannot exceed 200 characters");
        }

        // Validate Content
        if (string.IsNullOrWhiteSpace(createDto.Content))
        {
            errors.GetOrAdd("Content").Add("Content is required");
        }
        else if (createDto.Content.Length > 10000)
        {
            errors.GetOrAdd("Content").Add("Content cannot exceed 10,000 characters");
        }

        // Validate Embedding
        if (!ValidateEmbedding(createDto.Embedding))
        {
            var errorMessage = createDto.Embedding == null 
                ? "Embedding is required for memory creation" 
                : $"Embedding must have between {MinEmbeddingDimensions} and {MaxEmbeddingDimensions} dimensions and contain valid values";
            errors.GetOrAdd("Embedding").Add(errorMessage);
        }

        // Validate Type
        if (!Enum.IsDefined(typeof(MemoryType), createDto.Type))
        {
            errors.GetOrAdd("Type").Add("Invalid memory type specified");
        }

        // Validate Metadata size (prevent excessively large metadata)
        if (createDto.Metadata.Count > 50)
        {
            errors.GetOrAdd("Metadata").Add("Metadata cannot contain more than 50 key-value pairs");
        }

        if (errors.Any())
        {
            var validationErrors = errors.ToDictionary(
                kvp => kvp.Key, 
                kvp => kvp.Value.ToArray()
            );
            throw new MemoryValidationException(validationErrors);
        }
    }

    /// <summary>
    /// Validates update memory request for business rules.
    /// </summary>
    private void ValidateUpdateRequest(UpdateMemoryDto updateDto)
    {
        var errors = new Dictionary<string, List<string>>();

        // Validate Content if provided
        if (!string.IsNullOrEmpty(updateDto.Content) && updateDto.Content.Length > 10000)
        {
            errors.GetOrAdd("Content").Add("Content cannot exceed 10,000 characters");
        }

        // Validate Embedding if provided
        if (updateDto.Embedding != null && !ValidateEmbedding(updateDto.Embedding))
        {
            var errorMessage = $"Embedding must have between {MinEmbeddingDimensions} and {MaxEmbeddingDimensions} dimensions and contain valid values";
            errors.GetOrAdd("Embedding").Add(errorMessage);
        }

        // Validate Type if provided
        if (updateDto.Type.HasValue && !Enum.IsDefined(typeof(MemoryType), updateDto.Type.Value))
        {
            errors.GetOrAdd("Type").Add("Invalid memory type specified");
        }

        // Validate Metadata size if provided
        if (updateDto.Metadata != null && updateDto.Metadata.Count > 50)
        {
            errors.GetOrAdd("Metadata").Add("Metadata cannot contain more than 50 key-value pairs");
        }

        if (errors.Any())
        {
            var validationErrors = errors.ToDictionary(
                kvp => kvp.Key, 
                kvp => kvp.Value.ToArray()
            );
            throw new MemoryValidationException(validationErrors);
        }
    }

    /// <summary>
    /// Maps Memory entity to MemoryResponseDto.
    /// </summary>
    private static MemoryResponseDto MapToResponseDto(Memory memory)
    {
        return new MemoryResponseDto
        {
            Id = memory.Id,
            ProjectId = memory.ProjectId,
            Content = memory.Content,
            Embedding = memory.Embedding?.ToArray(),
            Type = memory.Type,
            Created = memory.Created,
            LastAccessed = memory.LastAccessed,
            Metadata = memory.Metadata,
            IsDeprecated = memory.IsDeprecated
        };
    }
}

/// <summary>
/// Extension method to simplify adding errors to validation dictionary.
/// </summary>
internal static class DictionaryExtensions
{
    internal static List<string> GetOrAdd(this Dictionary<string, List<string>> dictionary, string key)
    {
        if (!dictionary.TryGetValue(key, out var list))
        {
            list = new List<string>();
            dictionary[key] = list;
        }
        return list;
    }
}