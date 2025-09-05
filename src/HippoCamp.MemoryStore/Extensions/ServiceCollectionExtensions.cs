using HippoCamp.MemoryStore.Middleware;
using HippoCamp.MemoryStore.Validation;

namespace HippoCamp.MemoryStore.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to register Memory Store services.
/// Provides centralized configuration for dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all validation services for the Memory Store.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="options">Optional validation configuration options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddMemoryValidation(
        this IServiceCollection services, 
        Action<ValidationOptions>? options = null)
    {
        var validationOptions = new ValidationOptions();
        options?.Invoke(validationOptions);

        // Register validation options
        services.AddSingleton(validationOptions);

        // Register memory validator
        services.AddScoped<MemoryValidator>();

        // Register embedding validator with configuration
        services.AddScoped<EmbeddingValidator>(provider =>
        {
            var opts = provider.GetRequiredService<ValidationOptions>();
            return new EmbeddingValidator(
                opts.DefaultEmbeddingModel, 
                opts.EnforceNormalization);
        });

        return services;
    }

    /// <summary>
    /// Registers exception handling middleware and related services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddMemoryErrorHandling(this IServiceCollection services)
    {
        // Register the global exception middleware as a service
        // This allows dependency injection into the middleware
        services.AddScoped<GlobalExceptionMiddleware>();

        return services;
    }

    /// <summary>
    /// Registers all Memory Store core services including validation and error handling.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="validationOptions">Optional validation configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddMemoryStore(
        this IServiceCollection services,
        Action<ValidationOptions>? validationOptions = null)
    {
        // Add validation services
        services.AddMemoryValidation(validationOptions);

        // Add error handling
        services.AddMemoryErrorHandling();

        // Add logging for Memory Store components
        services.AddLogging(logging =>
        {
            logging.AddConsole();
            logging.AddDebug();
        });

        return services;
    }

    /// <summary>
    /// Registers Memory Store services with custom embedding validation configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="embeddingModel">The default embedding model to validate against.</param>
    /// <param name="enforceNormalization">Whether to enforce vector normalization.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddMemoryStoreWithEmbeddings(
        this IServiceCollection services,
        string embeddingModel = "default",
        bool enforceNormalization = false)
    {
        return services.AddMemoryStore(options =>
        {
            options.DefaultEmbeddingModel = embeddingModel;
            options.EnforceNormalization = enforceNormalization;
        });
    }

    /// <summary>
    /// Registers Memory Store services with production-ready configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddMemoryStoreProduction(this IServiceCollection services)
    {
        return services.AddMemoryStore(options =>
        {
            options.DefaultEmbeddingModel = "openai-text-embedding-3-small";
            options.EnforceNormalization = true;
            options.EnableDetailedValidation = false; // Less verbose for production
            options.MaxContentSizeKB = 10;
            options.MaxMetadataSizeKB = 5;
        });
    }

    /// <summary>
    /// Registers Memory Store services with development-friendly configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddMemoryStoreDevelopment(this IServiceCollection services)
    {
        return services.AddMemoryStore(options =>
        {
            options.DefaultEmbeddingModel = "default";
            options.EnforceNormalization = false;
            options.EnableDetailedValidation = true; // More verbose for development
            options.MaxContentSizeKB = 10;
            options.MaxMetadataSizeKB = 5;
        });
    }
}

/// <summary>
/// Configuration options for Memory Store validation services.
/// </summary>
public class ValidationOptions
{
    /// <summary>
    /// Gets or sets the default embedding model for validation.
    /// </summary>
    public string DefaultEmbeddingModel { get; set; } = "default";

    /// <summary>
    /// Gets or sets whether to enforce vector normalization.
    /// </summary>
    public bool EnforceNormalization { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to enable detailed validation error messages.
    /// </summary>
    public bool EnableDetailedValidation { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum content size in KB.
    /// </summary>
    public int MaxContentSizeKB { get; set; } = 10;

    /// <summary>
    /// Gets or sets the maximum metadata size in KB.
    /// </summary>
    public int MaxMetadataSizeKB { get; set; } = 5;

    /// <summary>
    /// Gets or sets the maximum ProjectId length.
    /// </summary>
    public int MaxProjectIdLength { get; set; } = 200;

    /// <summary>
    /// Gets or sets custom embedding model configurations.
    /// </summary>
    public Dictionary<string, EmbeddingModelConfig> CustomEmbeddingModels { get; set; } = new();

    /// <summary>
    /// Gets or sets whether to validate suspicious content patterns.
    /// </summary>
    public bool ValidateSuspiciousContent { get; set; } = true;

    /// <summary>
    /// Gets or sets the list of suspicious content patterns to check for.
    /// </summary>
    public string[] SuspiciousPatterns { get; set; } = new[]
    {
        "<script",
        "javascript:",
        "data:text/html",
        "eval(",
        "setTimeout(",
        "setInterval("
    };

    /// <summary>
    /// Adds a custom embedding model configuration.
    /// </summary>
    /// <param name="modelName">The model name.</param>
    /// <param name="dimensions">Expected dimensions.</param>
    /// <param name="minValue">Minimum value range.</param>
    /// <param name="maxValue">Maximum value range.</param>
    /// <param name="zeroThreshold">Zero threshold for sparsity calculations.</param>
    /// <returns>The validation options for chaining.</returns>
    public ValidationOptions AddEmbeddingModel(
        string modelName, 
        int dimensions, 
        float minValue = -2.0f, 
        float maxValue = 2.0f, 
        float zeroThreshold = 1e-8f)
    {
        CustomEmbeddingModels[modelName] = new EmbeddingModelConfig(
            dimensions, 
            minValue, 
            maxValue, 
            zeroThreshold);

        return this;
    }

    /// <summary>
    /// Configures for OpenAI embedding models.
    /// </summary>
    /// <param name="modelVersion">The OpenAI model version (ada-002, text-embedding-3-small, text-embedding-3-large).</param>
    /// <returns>The validation options for chaining.</returns>
    public ValidationOptions UseOpenAIEmbeddings(string modelVersion = "text-embedding-3-small")
    {
        DefaultEmbeddingModel = $"openai-{modelVersion}";
        EnforceNormalization = true;
        
        return this;
    }

    /// <summary>
    /// Configures for production environment with strict validation.
    /// </summary>
    /// <returns>The validation options for chaining.</returns>
    public ValidationOptions UseProductionDefaults()
    {
        EnableDetailedValidation = false;
        ValidateSuspiciousContent = true;
        EnforceNormalization = true;
        MaxContentSizeKB = 10;
        MaxMetadataSizeKB = 5;
        
        return this;
    }

    /// <summary>
    /// Configures for development environment with relaxed validation.
    /// </summary>
    /// <returns>The validation options for chaining.</returns>
    public ValidationOptions UseDevelopmentDefaults()
    {
        EnableDetailedValidation = true;
        ValidateSuspiciousContent = false;
        EnforceNormalization = false;
        MaxContentSizeKB = 50; // More generous for development
        MaxMetadataSizeKB = 10;
        
        return this;
    }
}