using HippoCamp.MemoryStore.DTOs;
using HippoCamp.MemoryStore.Models;

namespace HippoCamp.MemoryStore.Services;

/// <summary>
/// Service interface for memory CRUD operations.
/// Handles all memory operations including creation, retrieval, updates, and soft deletion.
/// </summary>
public interface IMemoryService
{
    /// <summary>
    /// Creates a new memory with embeddings and metadata.
    /// </summary>
    /// <param name="createDto">Memory creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created memory response</returns>
    Task<MemoryResponseDto> CreateAsync(CreateMemoryDto createDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a memory by its unique identifier.
    /// </summary>
    /// <param name="id">Memory unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Memory response if found, null if not found or deleted</returns>
    Task<MemoryResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all non-deleted memories for a specific project.
    /// </summary>
    /// <param name="projectId">Project identifier</param>
    /// <param name="includeDeprecated">Whether to include deprecated memories</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of memory responses</returns>
    Task<List<MemoryResponseDto>> GetByProjectAsync(string projectId, bool includeDeprecated = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing memory with new content, metadata, or deprecation status.
    /// </summary>
    /// <param name="id">Memory unique identifier</param>
    /// <param name="updateDto">Memory update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated memory response if successful, null if memory not found</returns>
    Task<MemoryResponseDto?> UpdateAsync(Guid id, UpdateMemoryDto updateDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs soft delete on a memory by setting IsDeleted flag.
    /// Memory remains in database for retention period but is excluded from queries.
    /// </summary>
    /// <param name="id">Memory unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if memory was deleted, false if memory not found</returns>
    Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if an embedding array has consistent dimensions and valid values.
    /// </summary>
    /// <param name="embedding">Embedding array to validate</param>
    /// <returns>True if embedding is valid</returns>
    bool ValidateEmbedding(float[]? embedding);
}