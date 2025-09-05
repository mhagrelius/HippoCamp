using System.Numerics;
using Pgvector;

namespace HippoCamp.MemoryStore.Validation;

/// <summary>
/// Provides specialized validation for embedding vectors.
/// Ensures dimension consistency, value ranges, and vector quality.
/// </summary>
public class EmbeddingValidator
{
    // Configuration for different embedding models
    private static readonly Dictionary<string, EmbeddingModelConfig> _modelConfigs = new()
    {
        ["openai-ada-002"] = new EmbeddingModelConfig(1536, -1.0f, 1.0f, 1e-6f),
        ["openai-text-embedding-3-small"] = new EmbeddingModelConfig(1536, -1.0f, 1.0f, 1e-6f),
        ["openai-text-embedding-3-large"] = new EmbeddingModelConfig(3072, -1.0f, 1.0f, 1e-6f),
        ["cohere-embed-english-v3.0"] = new EmbeddingModelConfig(1024, -1.0f, 1.0f, 1e-6f),
        ["sentence-transformers-all-minilm-l6-v2"] = new EmbeddingModelConfig(384, -1.0f, 1.0f, 1e-6f),
        ["default"] = new EmbeddingModelConfig(1536, -2.0f, 2.0f, 1e-8f) // Flexible default
    };

    private readonly string _defaultModelName;
    private readonly bool _enforceNormalization;

    /// <summary>
    /// Initializes a new instance of the EmbeddingValidator.
    /// </summary>
    /// <param name="defaultModelName">The default embedding model to validate against.</param>
    /// <param name="enforceNormalization">Whether to enforce vector normalization.</param>
    public EmbeddingValidator(string defaultModelName = "default", bool enforceNormalization = false)
    {
        _defaultModelName = defaultModelName;
        _enforceNormalization = enforceNormalization;
    }

    /// <summary>
    /// Validates an embedding vector array.
    /// </summary>
    /// <param name="embedding">The embedding vector to validate.</param>
    /// <param name="modelName">Optional model name for specific validation rules.</param>
    /// <returns>Validation result with any errors found.</returns>
    public ValidationResult ValidateEmbedding(float[] embedding, string? modelName = null)
    {
        var result = new ValidationResult();
        var model = modelName ?? _defaultModelName;

        if (embedding == null)
        {
            result.AddError("Embedding", "Embedding cannot be null");
            return result;
        }

        if (embedding.Length == 0)
        {
            result.AddError("Embedding", "Embedding cannot be empty");
            return result;
        }

        var config = GetModelConfig(model);

        // Validate dimensions
        ValidateDimensions(embedding, config, result);

        // Validate value ranges
        ValidateValueRanges(embedding, config, result);

        // Validate for NaN and infinity
        ValidateFiniteValues(embedding, result);

        // Validate vector quality
        ValidateVectorQuality(embedding, config, result);

        // Validate normalization if required
        if (_enforceNormalization)
        {
            ValidateNormalization(embedding, result);
        }

        return result;
    }

    /// <summary>
    /// Validates a pgvector Vector object.
    /// </summary>
    /// <param name="vector">The pgvector Vector to validate.</param>
    /// <param name="modelName">Optional model name for specific validation rules.</param>
    /// <returns>Validation result with any errors found.</returns>
    public ValidationResult ValidateVector(Pgvector.Vector vector, string? modelName = null)
    {
        if (vector == null)
        {
            var result = new ValidationResult();
            result.AddError("Embedding", "Vector cannot be null");
            return result;
        }

        // Convert Vector to float array for validation
        var floatArray = vector.ToArray();
        return ValidateEmbedding(floatArray, modelName);
    }

    /// <summary>
    /// Validates consistency between multiple embeddings (same dimensions and similar characteristics).
    /// </summary>
    /// <param name="embeddings">Collection of embeddings to validate for consistency.</param>
    /// <param name="modelName">Optional model name for validation rules.</param>
    /// <returns>Validation result with any errors found.</returns>
    public ValidationResult ValidateEmbeddingConsistency(IEnumerable<float[]> embeddings, string? modelName = null)
    {
        var result = new ValidationResult();
        var embeddingList = embeddings.ToList();

        if (embeddingList.Count == 0)
        {
            return result; // No embeddings to validate
        }

        var firstEmbedding = embeddingList.First();
        if (firstEmbedding == null)
        {
            result.AddError("Embeddings", "First embedding in collection is null");
            return result;
        }

        var expectedDimensions = firstEmbedding.Length;

        for (int i = 0; i < embeddingList.Count; i++)
        {
            var embedding = embeddingList[i];
            if (embedding == null)
            {
                result.AddError("Embeddings", $"Embedding at index {i} is null");
                continue;
            }

            if (embedding.Length != expectedDimensions)
            {
                result.AddError("Embeddings", 
                    $"Embedding at index {i} has {embedding.Length} dimensions, expected {expectedDimensions}");
            }

            // Validate each individual embedding
            var individualResult = ValidateEmbedding(embedding, modelName);
            if (!individualResult.IsValid)
            {
                foreach (var error in individualResult.GetAllErrors())
                {
                    result.AddError("Embeddings", $"Embedding at index {i}: {error}");
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Calculates similarity between embeddings and validates they're reasonable.
    /// </summary>
    /// <param name="embedding1">First embedding.</param>
    /// <param name="embedding2">Second embedding.</param>
    /// <returns>Validation result including similarity score.</returns>
    public EmbeddingSimilarityResult ValidateSimilarity(float[] embedding1, float[] embedding2)
    {
        var result = new EmbeddingSimilarityResult();

        // Validate both embeddings first
        var validation1 = ValidateEmbedding(embedding1);
        var validation2 = ValidateEmbedding(embedding2);

        if (!validation1.IsValid)
        {
            result.AddError("Embedding1", "First embedding is invalid");
            foreach (var error in validation1.GetAllErrors())
            {
                result.AddError("Embedding1", error);
            }
        }

        if (!validation2.IsValid)
        {
            result.AddError("Embedding2", "Second embedding is invalid");
            foreach (var error in validation2.GetAllErrors())
            {
                result.AddError("Embedding2", error);
            }
        }

        if (!result.IsValid)
        {
            return result;
        }

        if (embedding1.Length != embedding2.Length)
        {
            result.AddError("Similarity", $"Embeddings have different dimensions: {embedding1.Length} vs {embedding2.Length}");
            return result;
        }

        // Calculate cosine similarity
        try
        {
            result.CosineSimilarity = CalculateCosineSimilarity(embedding1, embedding2);
            
            // Validate similarity is in expected range
            if (result.CosineSimilarity < -1.1f || result.CosineSimilarity > 1.1f)
            {
                result.AddError("Similarity", $"Cosine similarity out of valid range: {result.CosineSimilarity}");
            }
        }
        catch (Exception ex)
        {
            result.AddError("Similarity", $"Error calculating similarity: {ex.Message}");
        }

        return result;
    }

    /// <summary>
    /// Gets the model configuration for the specified model name.
    /// </summary>
    /// <param name="modelName">The model name.</param>
    /// <returns>The model configuration.</returns>
    private EmbeddingModelConfig GetModelConfig(string modelName)
    {
        return _modelConfigs.ContainsKey(modelName) 
            ? _modelConfigs[modelName] 
            : _modelConfigs["default"];
    }

    /// <summary>
    /// Validates embedding dimensions against model configuration.
    /// </summary>
    private void ValidateDimensions(float[] embedding, EmbeddingModelConfig config, ValidationResult result)
    {
        if (embedding.Length != config.ExpectedDimensions && config.ExpectedDimensions > 0)
        {
            result.AddError("Embedding", 
                $"Invalid embedding dimension: expected {config.ExpectedDimensions}, got {embedding.Length}");
        }
    }

    /// <summary>
    /// Validates embedding values are within acceptable ranges.
    /// </summary>
    private void ValidateValueRanges(float[] embedding, EmbeddingModelConfig config, ValidationResult result)
    {
        for (int i = 0; i < embedding.Length; i++)
        {
            var value = embedding[i];
            if (value < config.MinValue || value > config.MaxValue)
            {
                result.AddError("Embedding", 
                    $"Value at index {i} ({value}) is outside valid range [{config.MinValue}, {config.MaxValue}]");
                break; // Don't spam with too many range errors
            }
        }
    }

    /// <summary>
    /// Validates embedding values are finite (not NaN or infinity).
    /// </summary>
    private void ValidateFiniteValues(float[] embedding, ValidationResult result)
    {
        for (int i = 0; i < embedding.Length; i++)
        {
            var value = embedding[i];
            if (float.IsNaN(value))
            {
                result.AddError("Embedding", $"Value at index {i} is NaN");
                return; // Stop on first invalid value
            }
            if (float.IsInfinity(value))
            {
                result.AddError("Embedding", $"Value at index {i} is infinity");
                return; // Stop on first invalid value
            }
        }
    }

    /// <summary>
    /// Validates overall vector quality (magnitude, sparsity, etc.).
    /// </summary>
    private void ValidateVectorQuality(float[] embedding, EmbeddingModelConfig config, ValidationResult result)
    {
        // Check for all-zero vector
        if (embedding.All(x => Math.Abs(x) < config.ZeroThreshold))
        {
            result.AddError("Embedding", "Embedding is effectively a zero vector");
            return;
        }

        // Calculate magnitude
        var magnitude = (float)Math.Sqrt(embedding.Sum(x => x * x));
        
        // Check for extremely small or large magnitudes
        if (magnitude < config.ZeroThreshold)
        {
            result.AddError("Embedding", $"Embedding magnitude too small: {magnitude}");
        }
        else if (magnitude > 100.0f) // Reasonable upper bound for most embeddings
        {
            result.AddError("Embedding", $"Embedding magnitude unusually large: {magnitude}");
        }

        // Check sparsity (too many zeros might indicate a problem)
        var zeroCount = embedding.Count(x => Math.Abs(x) < config.ZeroThreshold);
        var sparsityRatio = (float)zeroCount / embedding.Length;
        
        if (sparsityRatio > 0.95f) // More than 95% zeros
        {
            result.AddError("Embedding", $"Embedding is too sparse: {sparsityRatio:P1} zeros");
        }
    }

    /// <summary>
    /// Validates vector normalization (unit vector).
    /// </summary>
    private void ValidateNormalization(float[] embedding, ValidationResult result)
    {
        var magnitude = (float)Math.Sqrt(embedding.Sum(x => x * x));
        var normalizedMagnitude = Math.Abs(magnitude - 1.0f);
        
        if (normalizedMagnitude > 0.01f) // Allow small tolerance
        {
            result.AddError("Embedding", $"Embedding is not normalized: magnitude = {magnitude}");
        }
    }

    /// <summary>
    /// Calculates cosine similarity between two embeddings.
    /// </summary>
    private float CalculateCosineSimilarity(float[] embedding1, float[] embedding2)
    {
        var dotProduct = 0.0f;
        var norm1 = 0.0f;
        var norm2 = 0.0f;

        for (int i = 0; i < embedding1.Length; i++)
        {
            dotProduct += embedding1[i] * embedding2[i];
            norm1 += embedding1[i] * embedding1[i];
            norm2 += embedding2[i] * embedding2[i];
        }

        if (norm1 == 0.0f || norm2 == 0.0f)
        {
            return 0.0f;
        }

        return dotProduct / ((float)Math.Sqrt(norm1) * (float)Math.Sqrt(norm2));
    }
}

/// <summary>
/// Configuration for different embedding models.
/// </summary>
public record EmbeddingModelConfig(
    int ExpectedDimensions,
    float MinValue,
    float MaxValue,
    float ZeroThreshold
);

/// <summary>
/// Result of embedding similarity validation.
/// </summary>
public class EmbeddingSimilarityResult : ValidationResult
{
    /// <summary>
    /// The calculated cosine similarity between embeddings.
    /// </summary>
    public float CosineSimilarity { get; set; }
}