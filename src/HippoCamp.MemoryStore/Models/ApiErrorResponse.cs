namespace HippoCamp.MemoryStore.Models;

/// <summary>
/// Standardized error response format for Memory Store API.
/// Provides consistent error information structure for client applications.
/// </summary>
public class ApiErrorResponse
{
    /// <summary>
    /// High-level error message describing what went wrong.
    /// </summary>
    public string Error { get; set; } = string.Empty;

    /// <summary>
    /// Detailed error information or additional context.
    /// Optional - may be null for simple errors.
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Field-specific validation errors.
    /// Key is the field name, value is array of error messages for that field.
    /// Optional - only populated for validation failures.
    /// </summary>
    public Dictionary<string, string[]>? ValidationErrors { get; set; }

    /// <summary>
    /// HTTP status code associated with this error.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Timestamp when the error occurred.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Creates a simple error response with message and status code.
    /// </summary>
    /// <param name="error">Error message</param>
    /// <param name="statusCode">HTTP status code</param>
    /// <returns>ApiErrorResponse instance</returns>
    public static ApiErrorResponse Create(string error, int statusCode)
    {
        return new ApiErrorResponse
        {
            Error = error,
            StatusCode = statusCode
        };
    }

    /// <summary>
    /// Creates an error response with detailed information.
    /// </summary>
    /// <param name="error">Error message</param>
    /// <param name="details">Detailed error information</param>
    /// <param name="statusCode">HTTP status code</param>
    /// <returns>ApiErrorResponse instance</returns>
    public static ApiErrorResponse Create(string error, string details, int statusCode)
    {
        return new ApiErrorResponse
        {
            Error = error,
            Details = details,
            StatusCode = statusCode
        };
    }

    /// <summary>
    /// Creates a validation error response with field-specific errors.
    /// </summary>
    /// <param name="validationErrors">Field validation errors</param>
    /// <param name="statusCode">HTTP status code (typically 400)</param>
    /// <returns>ApiErrorResponse instance</returns>
    public static ApiErrorResponse CreateValidationError(Dictionary<string, string[]> validationErrors, int statusCode = 400)
    {
        return new ApiErrorResponse
        {
            Error = "Validation failed",
            ValidationErrors = validationErrors,
            StatusCode = statusCode
        };
    }
}