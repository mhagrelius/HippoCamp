using System.Net;
using System.Text.Json;
using HippoCamp.MemoryStore.Models;
using HippoCamp.MemoryStore.Validation;

namespace HippoCamp.MemoryStore.Middleware;

/// <summary>
/// Global exception handling middleware for the Memory Store API.
/// Catches unhandled exceptions and converts them to standardized error responses.
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    /// Initializes a new instance of the GlobalExceptionMiddleware.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">Logger instance for error logging.</param>
    /// <param name="environment">Web host environment for determining error detail level.</param>
    public GlobalExceptionMiddleware(
        RequestDelegate next, 
        ILogger<GlobalExceptionMiddleware> logger, 
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    /// <summary>
    /// Invokes the middleware to handle the HTTP request.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred during request processing");
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Handles exceptions by converting them to appropriate HTTP responses.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = CreateErrorResponse(exception);
        
        context.Response.StatusCode = response.StatusCode;
        context.Response.ContentType = "application/json";

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        };

        var jsonResponse = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(jsonResponse);
    }

    /// <summary>
    /// Creates an appropriate error response based on the exception type.
    /// </summary>
    /// <param name="exception">The exception to convert to an error response.</param>
    /// <returns>An ApiErrorResponse object.</returns>
    private ApiErrorResponse CreateErrorResponse(Exception exception)
    {
        return exception switch
        {
            ValidationException validationEx => HandleValidationException(validationEx),
            ArgumentNullException argNullEx => HandleArgumentNullException(argNullEx),
            ArgumentException argEx => HandleArgumentException(argEx),
            InvalidOperationException invalidOpEx => HandleInvalidOperationException(invalidOpEx),
            NotSupportedException notSupportedEx => HandleNotSupportedException(notSupportedEx),
            TimeoutException timeoutEx => HandleTimeoutException(timeoutEx),
            UnauthorizedAccessException unauthorizedEx => HandleUnauthorizedAccessException(unauthorizedEx),
            KeyNotFoundException notFoundEx => HandleNotFoundException(notFoundEx),
            JsonException jsonEx => HandleJsonException(jsonEx),
            _ => HandleGenericException(exception)
        };
    }

    /// <summary>
    /// Handles validation exceptions from the validation layer.
    /// </summary>
    private ApiErrorResponse HandleValidationException(ValidationException exception)
    {
        var validationErrors = new Dictionary<string, string[]>();
        
        if (exception.ValidationResult != null && !exception.ValidationResult.IsValid)
        {
            foreach (var error in exception.ValidationResult.Errors)
            {
                validationErrors[error.Key] = error.Value.ToArray();
            }
        }

        return ApiErrorResponse.CreateValidationError(validationErrors, (int)HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// Handles argument null exceptions.
    /// </summary>
    private ApiErrorResponse HandleArgumentNullException(ArgumentNullException exception)
    {
        var error = "Required parameter is null";
        var details = _environment.IsDevelopment() 
            ? $"Parameter '{exception.ParamName}' cannot be null" 
            : null;

        return ApiErrorResponse.Create(error, details, (int)HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// Handles argument exceptions.
    /// </summary>
    private ApiErrorResponse HandleArgumentException(ArgumentException exception)
    {
        var error = "Invalid parameter value";
        var details = _environment.IsDevelopment() 
            ? exception.Message 
            : "One or more parameters have invalid values";

        return ApiErrorResponse.Create(error, details, (int)HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// Handles invalid operation exceptions.
    /// </summary>
    private ApiErrorResponse HandleInvalidOperationException(InvalidOperationException exception)
    {
        var error = "Operation not allowed";
        var details = _environment.IsDevelopment() 
            ? exception.Message 
            : "The requested operation cannot be performed in the current state";

        return ApiErrorResponse.Create(error, details, (int)HttpStatusCode.Conflict);
    }

    /// <summary>
    /// Handles not supported exceptions.
    /// </summary>
    private ApiErrorResponse HandleNotSupportedException(NotSupportedException exception)
    {
        var error = "Operation not supported";
        var details = _environment.IsDevelopment() 
            ? exception.Message 
            : "The requested operation is not supported";

        return ApiErrorResponse.Create(error, details, (int)HttpStatusCode.NotImplemented);
    }

    /// <summary>
    /// Handles timeout exceptions.
    /// </summary>
    private ApiErrorResponse HandleTimeoutException(TimeoutException exception)
    {
        var error = "Request timeout";
        var details = _environment.IsDevelopment() 
            ? exception.Message 
            : "The request took too long to process";

        return ApiErrorResponse.Create(error, details, (int)HttpStatusCode.RequestTimeout);
    }

    /// <summary>
    /// Handles unauthorized access exceptions.
    /// </summary>
    private ApiErrorResponse HandleUnauthorizedAccessException(UnauthorizedAccessException exception)
    {
        var error = "Access denied";
        var details = _environment.IsDevelopment() 
            ? exception.Message 
            : "You do not have permission to perform this operation";

        return ApiErrorResponse.Create(error, details, (int)HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// Handles key not found exceptions.
    /// </summary>
    private ApiErrorResponse HandleNotFoundException(KeyNotFoundException exception)
    {
        var error = "Resource not found";
        var details = _environment.IsDevelopment() 
            ? exception.Message 
            : "The requested resource could not be found";

        return ApiErrorResponse.Create(error, details, (int)HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Handles JSON serialization/deserialization exceptions.
    /// </summary>
    private ApiErrorResponse HandleJsonException(JsonException exception)
    {
        var error = "Invalid JSON format";
        var details = _environment.IsDevelopment() 
            ? exception.Message 
            : "The request contains invalid JSON data";

        return ApiErrorResponse.Create(error, details, (int)HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// Handles all other unhandled exceptions.
    /// </summary>
    private ApiErrorResponse HandleGenericException(Exception exception)
    {
        var error = "Internal server error";
        var details = _environment.IsDevelopment() 
            ? exception.Message 
            : "An unexpected error occurred while processing your request";

        return ApiErrorResponse.Create(error, details, (int)HttpStatusCode.InternalServerError);
    }
}

/// <summary>
/// Custom exception type for validation errors.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Gets the validation result containing detailed error information.
    /// </summary>
    public ValidationResult? ValidationResult { get; }

    /// <summary>
    /// Initializes a new instance of the ValidationException.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public ValidationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ValidationException with validation result.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="validationResult">The validation result with detailed errors.</param>
    public ValidationException(string message, ValidationResult validationResult) : base(message)
    {
        ValidationResult = validationResult;
    }

    /// <summary>
    /// Initializes a new instance of the ValidationException with inner exception.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Creates a ValidationException from a ValidationResult.
    /// </summary>
    /// <param name="validationResult">The validation result.</param>
    /// <returns>A new ValidationException instance.</returns>
    public static ValidationException FromValidationResult(ValidationResult validationResult)
    {
        var message = validationResult.IsValid 
            ? "Validation failed" 
            : string.Join("; ", validationResult.GetAllErrors());

        return new ValidationException(message, validationResult);
    }
}