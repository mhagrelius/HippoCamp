using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using HippoCamp.MemoryStore.DTOs;
using HippoCamp.MemoryStore.Models;

namespace HippoCamp.MemoryStore.Validation;

/// <summary>
/// Provides comprehensive validation for memory-related operations.
/// Validates content length, ProjectId format, metadata schema, and business rules.
/// </summary>
public class MemoryValidator
{
    private const int MaxContentLength = 10240; // 10KB
    private const int MaxProjectIdLength = 200;
    private const int MinProjectIdLength = 1;
    private const int MaxMetadataSize = 5120; // 5KB for metadata JSON

    /// <summary>
    /// Validates a CreateMemoryDto for creating new memories.
    /// </summary>
    /// <param name="dto">The memory creation data to validate.</param>
    /// <returns>Validation result with any errors found.</returns>
    public ValidationResult ValidateCreateMemory(CreateMemoryDto dto)
    {
        var result = new ValidationResult();

        if (dto == null)
        {
            result.AddError("Memory", "Memory data cannot be null");
            return result;
        }

        // Validate ProjectId
        ValidateProjectId(dto.ProjectId, result);

        // Validate Content
        ValidateContent(dto.Content, result);

        // Validate MemoryType
        ValidateMemoryType(dto.Type, result);

        // Validate Metadata
        ValidateMetadata(dto.Metadata, result);

        // Validate Embedding if provided
        if (dto.Embedding != null)
        {
            ValidateEmbeddingBasic(dto.Embedding, result);
        }

        return result;
    }

    /// <summary>
    /// Validates an UpdateMemoryDto for updating existing memories.
    /// </summary>
    /// <param name="dto">The memory update data to validate.</param>
    /// <returns>Validation result with any errors found.</returns>
    public ValidationResult ValidateUpdateMemory(UpdateMemoryDto dto)
    {
        var result = new ValidationResult();

        if (dto == null)
        {
            result.AddError("Memory", "Memory data cannot be null");
            return result;
        }

        // Validate Content if provided
        if (!string.IsNullOrEmpty(dto.Content))
        {
            ValidateContent(dto.Content, result);
        }

        // Validate MemoryType if provided
        if (dto.Type.HasValue)
        {
            ValidateMemoryType(dto.Type.Value, result);
        }

        // Validate Metadata if provided
        if (dto.Metadata != null)
        {
            ValidateMetadata(dto.Metadata, result);
        }

        // Validate Embedding if provided
        if (dto.Embedding != null)
        {
            ValidateEmbeddingBasic(dto.Embedding, result);
        }

        return result;
    }

    /// <summary>
    /// Validates ProjectId format and constraints.
    /// </summary>
    /// <param name="projectId">The project identifier to validate.</param>
    /// <param name="result">The validation result to add errors to.</param>
    private void ValidateProjectId(string projectId, ValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(projectId))
        {
            result.AddError("ProjectId", "ProjectId is required");
            return;
        }

        if (projectId.Length < MinProjectIdLength)
        {
            result.AddError("ProjectId", $"ProjectId must be at least {MinProjectIdLength} character(s) long");
        }

        if (projectId.Length > MaxProjectIdLength)
        {
            result.AddError("ProjectId", $"ProjectId cannot exceed {MaxProjectIdLength} characters");
        }

        // Validate ProjectId format - should be alphanumeric with allowed special characters
        if (!IsValidProjectIdFormat(projectId))
        {
            result.AddError("ProjectId", "ProjectId contains invalid characters. Only letters, numbers, hyphens, underscores, and dots are allowed");
        }
    }

    /// <summary>
    /// Validates memory content constraints.
    /// </summary>
    /// <param name="content">The memory content to validate.</param>
    /// <param name="result">The validation result to add errors to.</param>
    private void ValidateContent(string content, ValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            result.AddError("Content", "Content is required");
            return;
        }

        var contentBytes = System.Text.Encoding.UTF8.GetByteCount(content);
        if (contentBytes > MaxContentLength)
        {
            result.AddError("Content", $"Content cannot exceed {MaxContentLength / 1024}KB ({contentBytes} bytes provided)");
        }

        // Check for potentially harmful content patterns
        if (ContainsSuspiciousContent(content))
        {
            result.AddError("Content", "Content contains potentially harmful patterns");
        }
    }

    /// <summary>
    /// Validates memory type enum value.
    /// </summary>
    /// <param name="type">The memory type to validate.</param>
    /// <param name="result">The validation result to add errors to.</param>
    private void ValidateMemoryType(MemoryType type, ValidationResult result)
    {
        if (!Enum.IsDefined(typeof(MemoryType), type))
        {
            result.AddError("Type", $"Invalid memory type: {type}");
        }
    }

    /// <summary>
    /// Validates metadata dictionary constraints and schema.
    /// </summary>
    /// <param name="metadata">The metadata to validate.</param>
    /// <param name="result">The validation result to add errors to.</param>
    private void ValidateMetadata(Dictionary<string, object> metadata, ValidationResult result)
    {
        if (metadata == null)
            return;

        try
        {
            var json = JsonSerializer.Serialize(metadata);
            var jsonBytes = System.Text.Encoding.UTF8.GetByteCount(json);
            
            if (jsonBytes > MaxMetadataSize)
            {
                result.AddError("Metadata", $"Metadata cannot exceed {MaxMetadataSize / 1024}KB ({jsonBytes} bytes provided)");
            }

            // Validate metadata keys
            foreach (var key in metadata.Keys)
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    result.AddError("Metadata", "Metadata keys cannot be null or empty");
                }
                else if (key.Length > 100)
                {
                    result.AddError("Metadata", $"Metadata key '{key}' exceeds 100 character limit");
                }
                else if (!IsValidMetadataKey(key))
                {
                    result.AddError("Metadata", $"Metadata key '{key}' contains invalid characters");
                }
            }

            // Validate that values are JSON-serializable
            ValidateMetadataValues(metadata, result);
        }
        catch (JsonException ex)
        {
            result.AddError("Metadata", $"Metadata is not valid JSON: {ex.Message}");
        }
    }

    /// <summary>
    /// Basic validation for embedding arrays.
    /// </summary>
    /// <param name="embedding">The embedding array to validate.</param>
    /// <param name="result">The validation result to add errors to.</param>
    private void ValidateEmbeddingBasic(float[] embedding, ValidationResult result)
    {
        if (embedding == null || embedding.Length == 0)
        {
            result.AddError("Embedding", "Embedding cannot be null or empty when provided");
            return;
        }

        // Check for NaN or infinite values
        if (embedding.Any(x => float.IsNaN(x) || float.IsInfinity(x)))
        {
            result.AddError("Embedding", "Embedding contains invalid values (NaN or Infinity)");
        }

        // Basic dimension check - will be enhanced by EmbeddingValidator
        if (embedding.Length < 1 || embedding.Length > 4096)
        {
            result.AddError("Embedding", $"Embedding dimension must be between 1 and 4096 (provided: {embedding.Length})");
        }
    }

    /// <summary>
    /// Checks if ProjectId format is valid.
    /// </summary>
    /// <param name="projectId">The project ID to check.</param>
    /// <returns>True if format is valid, false otherwise.</returns>
    private bool IsValidProjectIdFormat(string projectId)
    {
        // Allow letters, numbers, hyphens, underscores, and dots
        return projectId.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_' || c == '.');
    }

    /// <summary>
    /// Checks if content contains suspicious patterns that might indicate malicious input.
    /// </summary>
    /// <param name="content">The content to check.</param>
    /// <returns>True if suspicious content is detected, false otherwise.</returns>
    private bool ContainsSuspiciousContent(string content)
    {
        var suspiciousPatterns = new[]
        {
            "<script",
            "javascript:",
            "data:text/html",
            "eval(",
            "setTimeout(",
            "setInterval("
        };

        var lowerContent = content.ToLowerInvariant();
        return suspiciousPatterns.Any(pattern => lowerContent.Contains(pattern));
    }

    /// <summary>
    /// Checks if metadata key format is valid.
    /// </summary>
    /// <param name="key">The metadata key to check.</param>
    /// <returns>True if key format is valid, false otherwise.</returns>
    private bool IsValidMetadataKey(string key)
    {
        // Allow letters, numbers, hyphens, underscores, dots, and colons for namespaced keys
        return key.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_' || c == '.' || c == ':');
    }

    /// <summary>
    /// Validates that metadata values are serializable and within acceptable limits.
    /// </summary>
    /// <param name="metadata">The metadata dictionary to validate.</param>
    /// <param name="result">The validation result to add errors to.</param>
    private void ValidateMetadataValues(Dictionary<string, object> metadata, ValidationResult result)
    {
        foreach (var kvp in metadata)
        {
            if (kvp.Value == null)
                continue;

            // Check string values
            if (kvp.Value is string stringValue && stringValue.Length > 1000)
            {
                result.AddError("Metadata", $"Metadata value for key '{kvp.Key}' exceeds 1000 character limit");
            }

            // Ensure value is JSON-serializable by attempting to serialize it
            try
            {
                JsonSerializer.Serialize(kvp.Value);
            }
            catch (JsonException)
            {
                result.AddError("Metadata", $"Metadata value for key '{kvp.Key}' is not JSON serializable");
            }
        }
    }
}

/// <summary>
/// Represents the result of a validation operation.
/// </summary>
public class ValidationResult
{
    private readonly Dictionary<string, List<string>> _errors = new();

    /// <summary>
    /// Gets whether the validation was successful (no errors).
    /// </summary>
    public bool IsValid => _errors.Count == 0;

    /// <summary>
    /// Gets all validation errors grouped by field name.
    /// </summary>
    public IReadOnlyDictionary<string, IReadOnlyList<string>> Errors =>
        _errors.ToDictionary(
            kvp => kvp.Key,
            kvp => (IReadOnlyList<string>)kvp.Value.AsReadOnly()
        );

    /// <summary>
    /// Adds a validation error for the specified field.
    /// </summary>
    /// <param name="field">The field name that has the error.</param>
    /// <param name="message">The error message.</param>
    public void AddError(string field, string message)
    {
        if (!_errors.ContainsKey(field))
        {
            _errors[field] = new List<string>();
        }
        _errors[field].Add(message);
    }

    /// <summary>
    /// Gets all error messages as a flat list.
    /// </summary>
    /// <returns>List of all error messages.</returns>
    public List<string> GetAllErrors()
    {
        return _errors.Values.SelectMany(errors => errors).ToList();
    }

    /// <summary>
    /// Gets errors for a specific field.
    /// </summary>
    /// <param name="field">The field name to get errors for.</param>
    /// <returns>List of error messages for the field.</returns>
    public IReadOnlyList<string> GetErrorsFor(string field)
    {
        return _errors.ContainsKey(field) 
            ? _errors[field].AsReadOnly() 
            : new List<string>().AsReadOnly();
    }
}