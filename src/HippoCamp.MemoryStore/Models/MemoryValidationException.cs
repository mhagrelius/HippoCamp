namespace HippoCamp.MemoryStore.Models;

/// <summary>
/// Custom exception for memory validation errors.
/// Used to provide structured validation error information.
/// </summary>
public class MemoryValidationException : Exception
{
    /// <summary>
    /// Field-specific validation errors.
    /// </summary>
    public Dictionary<string, string[]> ValidationErrors { get; }

    /// <summary>
    /// Creates a new validation exception with field errors.
    /// </summary>
    /// <param name="validationErrors">Field validation errors</param>
    public MemoryValidationException(Dictionary<string, string[]> validationErrors)
        : base("Memory validation failed")
    {
        ValidationErrors = validationErrors ?? new Dictionary<string, string[]>();
    }

    /// <summary>
    /// Creates a new validation exception with a single field error.
    /// </summary>
    /// <param name="fieldName">Name of the field with validation error</param>
    /// <param name="errorMessage">Error message for the field</param>
    public MemoryValidationException(string fieldName, string errorMessage)
        : base($"Validation failed for field '{fieldName}': {errorMessage}")
    {
        ValidationErrors = new Dictionary<string, string[]>
        {
            { fieldName, new[] { errorMessage } }
        };
    }

    /// <summary>
    /// Creates a new validation exception with multiple errors for a single field.
    /// </summary>
    /// <param name="fieldName">Name of the field with validation errors</param>
    /// <param name="errorMessages">Error messages for the field</param>
    public MemoryValidationException(string fieldName, params string[] errorMessages)
        : base($"Validation failed for field '{fieldName}': {string.Join(", ", errorMessages)}")
    {
        ValidationErrors = new Dictionary<string, string[]>
        {
            { fieldName, errorMessages }
        };
    }
}