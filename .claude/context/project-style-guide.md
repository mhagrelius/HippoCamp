---
created: 2025-09-04T22:32:50Z
last_updated: 2025-09-04T22:32:50Z
version: 1.0
author: Claude Code PM System
---

# Project Style Guide

## Code Style Standards

### C# Language Conventions

#### **Variable Declarations**
```csharp
// ✅ Correct: Use var for built-in and apparent types
var count = 10;
var name = "HippoCamp";
var users = new List<User>();

// ❌ Avoid: Explicit type when var is appropriate
int count = 10;
string name = "HippoCamp";
List<User> users = new List<User>();
```

#### **Naming Conventions**
```csharp
// ✅ Private fields: _camelCase with underscore prefix
private readonly string _connectionString;
private int _retryCount;

// ✅ Constants: PascalCase
public const string DefaultConnectionString = "...";
private const int MaxRetryAttempts = 3;

// ✅ Properties, methods, classes: PascalCase
public class UserService
{
    public string UserName { get; set; }
    public async Task<User> GetUserAsync(int userId) { }
}

// ✅ Parameters and local variables: camelCase
public void ProcessUser(string userName, int maxAttempts)
{
    var currentUser = GetCurrentUser();
}
```

#### **Namespace Declarations**
```csharp
// ✅ Required: File-scoped namespaces for all new files
namespace HippoCamp.Services;

public class UserService
{
    // Implementation
}

// ❌ Avoid: Traditional namespace declarations
namespace HippoCamp.Services
{
    public class UserService
    {
        // Implementation
    }
}
```

#### **Type Usage**
```csharp
// ✅ Use predefined types for locals and parameters
int count = users.Count;
string message = GetMessage();

// ✅ Use predefined types for member access
var maxValue = int.MaxValue;
var emptyString = string.Empty;

// ❌ Avoid: Using framework type names
Int32 count = users.Count;
String message = GetMessage();
```

### Code Structure & Formatting

#### **Brace Style**
```csharp
// ✅ Required: Braces on new lines (Allman style)
if (condition)
{
    DoSomething();
}
else
{
    DoSomethingElse();
}

// ✅ Required: Always use braces, even for single statements
if (condition)
{
    return;
}

// ❌ Avoid: No braces for single statements
if (condition)
    return;
```

#### **Newline Conventions**
- **Open braces**: Always on new line
- **else statements**: New line before else
- **catch/finally**: New line before catch and finally
- **Object initializers**: Members on new lines
- **Anonymous types**: Members on new lines

#### **Indentation**
- **C# files**: 4 spaces
- **JSON files**: 2 spaces  
- **XML/config files**: 2 spaces
- **PowerShell scripts**: 4 spaces
- **Shell scripts**: 4 spaces

### Engineering Principles

#### **Async/Await Patterns**
```csharp
// ✅ Preferred: Async methods with cancellation tokens
public async Task<User> GetUserAsync(int userId, CancellationToken cancellationToken = default)
{
    return await _repository.GetByIdAsync(userId, cancellationToken);
}

// ✅ Async all the way - don't block on async calls
public async Task<string> ProcessUserAsync(int userId)
{
    var user = await GetUserAsync(userId);
    return await FormatUserAsync(user);
}

// ❌ Avoid: Blocking on async calls
public string ProcessUser(int userId)
{
    var user = GetUserAsync(userId).Result; // Don't do this
    return FormatUserAsync(user).Result;
}
```

#### **Function Design**
```csharp
// ✅ Small, single-purpose functions
public async Task<bool> IsUserActiveAsync(int userId)
{
    var user = await GetUserAsync(userId);
    return user?.LastLoginDate > DateTime.Now.AddDays(-30);
}

// ✅ Clear, descriptive names
public async Task SendWelcomeEmailAsync(User user)
{
    var template = await GetEmailTemplateAsync("welcome");
    await _emailService.SendAsync(user.Email, template);
}
```

#### **Composition Over Inheritance**
```csharp
// ✅ Preferred: Composition and dependency injection
public class UserService
{
    private readonly IUserRepository _repository;
    private readonly IEmailService _emailService;

    public UserService(IUserRepository repository, IEmailService emailService)
    {
        _repository = repository;
        _emailService = emailService;
    }
}

// ✅ Interface segregation
public interface IUserReader
{
    Task<User> GetByIdAsync(int id);
}

public interface IUserWriter
{
    Task SaveAsync(User user);
}
```

#### **Immutability Patterns**
```csharp
// ✅ Immutable data structures
public record User(int Id, string Name, DateTime CreatedAt);

// ✅ Readonly collections
public IReadOnlyList<string> GetUserRoles() => _roles.AsReadOnly();

// ✅ Init-only properties
public class Configuration
{
    public string ConnectionString { get; init; } = "";
    public int TimeoutSeconds { get; init; } = 30;
}
```

#### **Constants and Magic Values**
```csharp
// ✅ Use constants instead of magic strings/numbers
public static class CacheKeys
{
    public const string UserPrefix = "user:";
    public const int DefaultCacheMinutes = 15;
}

public async Task<User> GetUserAsync(int userId)
{
    var cacheKey = $"{CacheKeys.UserPrefix}{userId}";
    return await _cache.GetAsync(cacheKey, TimeSpan.FromMinutes(CacheKeys.DefaultCacheMinutes));
}
```

#### **Exception Handling**
```csharp
// ✅ Use exceptions for exceptional conditions only
public async Task<User> GetUserAsync(int userId)
{
    var user = await _repository.FindByIdAsync(userId);
    if (user == null)
    {
        throw new UserNotFoundException($"User {userId} not found");
    }
    return user;
}

// ✅ Domain-specific exceptions
public class UserNotFoundException : Exception
{
    public UserNotFoundException(string message) : base(message) { }
}

// ❌ Avoid: Using exceptions for control flow
public bool TryParseUser(string data, out User user)
{
    // Don't throw exceptions in Try* methods
    user = null;
    return JsonSerializer.Deserialize<User>(data) != null;
}
```

## Testing Standards

### Test Structure

#### **xUnit with FluentAssertions**
```csharp
[Fact]
public async Task GetUserAsync_WithValidId_ReturnsUser()
{
    // Arrange
    var userId = 1;
    var expectedUser = new User(userId, "John Doe", DateTime.Now);
    _mockRepository.Setup(r => r.GetByIdAsync(userId, default))
                  .ReturnsAsync(expectedUser);

    // Act
    var result = await _userService.GetUserAsync(userId);

    // Assert
    result.Should().NotBeNull();
    result.Id.Should().Be(userId);
    result.Name.Should().Be("John Doe");
}
```

#### **Theory and Data-Driven Tests**
```csharp
[Theory]
[InlineData("john@example.com", true)]
[InlineData("invalid-email", false)]
[InlineData("", false)]
public void IsValidEmail_WithVariousInputs_ReturnsExpectedResult(string email, bool expected)
{
    // Act
    var result = EmailValidator.IsValid(email);

    // Assert
    result.Should().Be(expected);
}
```

#### **Test Organization**
```csharp
// ✅ One logical assertion per test
[Fact]
public async Task CreateUserAsync_WithValidData_SavesUser()
{
    // Test focuses on saving behavior only
    var user = new User(0, "John Doe", DateTime.Now);
    
    await _userService.CreateUserAsync(user);
    
    _mockRepository.Verify(r => r.SaveAsync(user), Times.Once);
}

// ✅ Separate test for different behaviors
[Fact]
public async Task CreateUserAsync_WithValidData_ReturnsCreatedUser()
{
    // Test focuses on return value only
    var user = new User(0, "John Doe", DateTime.Now);
    
    var result = await _userService.CreateUserAsync(user);
    
    result.Should().NotBeNull();
    result.Id.Should().BeGreaterThan(0);
}
```

### Test Best Practices

#### **Deterministic Tests**
```csharp
// ✅ Use fixed dates for deterministic tests
private static readonly DateTime FixedDate = new DateTime(2025, 1, 1);

[Fact]
public void CalculateAge_WithBirthDate_ReturnsCorrectAge()
{
    var birthDate = FixedDate.AddYears(-25);
    
    var age = AgeCalculator.Calculate(birthDate, FixedDate);
    
    age.Should().Be(25);
}

// ❌ Avoid: Non-deterministic tests
[Fact]
public void CalculateAge_WithBirthDate_ReturnsCorrectAge()
{
    var birthDate = DateTime.Now.AddYears(-25); // This will change!
    
    var age = AgeCalculator.Calculate(birthDate, DateTime.Now);
    
    age.Should().Be(25); // This might fail due to timing
}
```

#### **Mock Strategy**
```csharp
// ✅ Use real objects for simple classes
[Fact]
public void User_WithName_SetsNameProperty()
{
    var user = new User(1, "John Doe", DateTime.Now); // Real object
    
    user.Name.Should().Be("John Doe");
}

// ✅ Mock external dependencies and complex collaborators
private readonly Mock<IUserRepository> _mockRepository = new();
private readonly Mock<IEmailService> _mockEmailService = new();

[Fact]
public async Task SendWelcomeEmail_WithNewUser_CallsEmailService()
{
    var user = new User(1, "John", DateTime.Now);
    
    await _userService.SendWelcomeEmailAsync(user);
    
    _mockEmailService.Verify(e => e.SendAsync(user.Email, It.IsAny<string>()), Times.Once);
}
```

## File and Project Organization

### File Naming Conventions
- **C# classes**: PascalCase matching class name (e.g., `UserService.cs`)
- **Test files**: Class name + "Tests" (e.g., `UserServiceTests.cs`)
- **Interface files**: Interface name (e.g., `IUserRepository.cs`)
- **Configuration**: kebab-case (e.g., `appsettings.json`, `docker-compose.yml`)

### Directory Structure
```
src/
  HippoCamp.Domain/          # Domain models and business logic
  HippoCamp.Infrastructure/   # Data access and external services
  HippoCamp.Api/             # Web API controllers and endpoints
  HippoCamp.AppHost/         # Aspire orchestration

tests/
  HippoCamp.Domain.Tests/    # Domain layer tests
  HippoCamp.Infrastructure.Tests/  # Infrastructure tests
  HippoCamp.Api.Tests/       # API integration tests
```

## Documentation Standards

### Code Comments
```csharp
// ✅ Document public APIs
/// <summary>
/// Retrieves a user by their unique identifier.
/// </summary>
/// <param name="userId">The unique identifier of the user.</param>
/// <param name="cancellationToken">Token to cancel the operation.</param>
/// <returns>The user if found.</returns>
/// <exception cref="UserNotFoundException">Thrown when user is not found.</exception>
public async Task<User> GetUserAsync(int userId, CancellationToken cancellationToken = default)

// ✅ Explain complex business logic
// Calculate discount based on user tier and purchase history
// Platinum users get 15%, Gold get 10%, Silver get 5%
var discountPercentage = user.Tier switch
{
    UserTier.Platinum => 0.15m,
    UserTier.Gold => 0.10m,
    UserTier.Silver => 0.05m,
    _ => 0m
};

// ❌ Avoid: Obvious comments
var count = 0; // Initialize count to zero
```

### README and Documentation
- **Comprehensive README**: Clear setup and usage instructions
- **API Documentation**: Swagger/OpenAPI integration
- **Architecture Decision Records**: Document important architectural choices
- **Changelog**: Track all significant changes

This style guide ensures consistent, maintainable, and high-quality code across the HippoCamp project.