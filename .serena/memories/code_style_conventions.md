# Code Style and Conventions

## Language and Framework Standards
- **Target Framework**: .NET 9.0
- **Language Features**: C# 13 with nullable reference types enabled
- **Global Usings**: Enabled for common namespaces
- **File-scoped namespaces**: Required (enforced via .editorconfig)

## C# Coding Conventions (Based on ASP.NET Core Standards)

### Variable Declaration
- **Use `var`**: Required for built-in types and when type is apparent
- **No `this.` qualifier**: Avoid unless necessary for disambiguation
- **Predefined types**: Use `int` over `Int32`, `string` over `String`

### Code Structure
- **File-scoped namespaces**: Always use `namespace MyNamespace;` format
- **Braces**: Required even for single-line statements
- **Private fields**: Use `_camelCase` naming with underscore prefix
- **Constants**: Use `PascalCase` for constant fields

### Async and Modern Patterns
- **Async/await**: Preferred for I/O operations
- **Cancellation tokens**: Always pass and utilize cancellation tokens
- **Exceptions**: Limit use of exceptions for control flow
- **Functions**: Keep small and single-purpose
- **Composition over inheritance**: Prefer composition patterns
- **Immutability**: Prefer immutable data structures
- **Constants**: Use constants instead of magic strings/numbers

### Formatting Rules
- **Indentation**: 4 spaces for C# files, 2 spaces for JSON/XML
- **Line endings**: LF (Unix-style)
- **Charset**: UTF-8
- **Newlines**: Before open braces, else, catch, finally
- **Multiple blank lines**: Not allowed
- **Trailing whitespace**: Removed automatically

### Naming Conventions
- **Methods/Properties**: PascalCase
- **Parameters/Variables**: camelCase
- **Private fields**: _camelCase (with underscore)
- **Constants**: UPPER_CASE or PascalCase
- **Namespaces**: PascalCase.PascalCase

### Testing Conventions
- **Framework**: xUnit v3 with FluentAssertions and Moq
- **Test organization**: Keep test files small, break up by functionality
- **Test structure**: One logical assertion per test
- **Test naming**: Descriptive method names that read like documentation
- **Data-driven tests**: Use `[InlineData]` and `[Theory]` attributes
- **Test output**: Use `ITestOutputHelper` for debug output
- **Mocking**: Avoid over-mocking, use real objects for value types
- **Deterministic**: Tests should be deterministic and not depend on external state

### Aspire-Specific Conventions
- **AppHost**: Central orchestration point for all services
- **Service defaults**: Common configuration shared across services
- **Resource naming**: Use descriptive names for Aspire resources
- **Health checks**: Implement health checks for all services
- **Configuration**: Use strongly-typed configuration classes