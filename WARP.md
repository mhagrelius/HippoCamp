# WARP.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

# HippoCamp .NET 9 Aspire Development Guide

HippoCamp is a cloud-native .NET 9 application built with Microsoft .NET Aspire for orchestrating distributed services. This guide provides essential commands and architecture insights for productive development in the Warp terminal.

## Prerequisites

**Required:**
- .NET 9.0 SDK (9.0.304 or later)
- Docker Desktop or Podman (for container runtime)

**Verification:**
```bash
dotnet --info                    # Verify .NET 9 SDK
dotnet workload list             # Check Aspire workload (may be built-in for .NET 9)
docker version                  # Verify container runtime
```

**Optional Development Tools:**
- GitHub CLI (`gh`) for issue management and PR creation
- `aspire` CLI tool (optional, can use `dotnet run` directly)

## Quick Start

```bash
# Clone and setup
git clone https://github.com/mhagrelius/HippoCamp.git
cd HippoCamp
dotnet restore

# Run the application (Aspire Dashboard will open automatically)
dotnet run --project src/HippoCamp.AppHost

# Alternative: Use Aspire CLI (if installed)
aspire run

# Run tests
dotnet test
```

**Aspire Dashboard:** Opens automatically at `https://localhost:17134` (HTTPS) or `http://localhost:15170` (HTTP)

## Essential Development Commands

### Building & Restoring
```bash
dotnet restore                   # Restore NuGet packages
dotnet build                     # Build entire solution (Debug)
dotnet build -c Release          # Build for Release
dotnet clean                     # Clean build artifacts
```

### Running the Application
```bash
# Primary method: Run AppHost directly
dotnet run --project src/HippoCamp.AppHost

# Watch mode (auto-restart on changes)
dotnet watch --project src/HippoCamp.AppHost run

# With specific launch profile (if needed)
dotnet run --project src/HippoCamp.AppHost --launch-profile http

# Alternative: Aspire CLI (auto-discovers AppHost)
aspire run
```

### Testing
```bash
# Run all tests
dotnet test

# Run with verbose output
dotnet test -v detailed

# Run specific test project
dotnet test tests/HippoCamp.Tests

# Filter by test name pattern
dotnet test --filter "FullyQualifiedName~IntegrationTest"

# Run tests with code coverage
dotnet test --collect:"XPlat Code Coverage"
```

### User Secrets (Project has UserSecretsId configured)
```bash
# Set a secret for the AppHost project
dotnet user-secrets set "ApiKey" "your-secret-value" --project src/HippoCamp.AppHost

# List all secrets
dotnet user-secrets list --project src/HippoCamp.AppHost

# Clear all secrets
dotnet user-secrets clear --project src/HippoCamp.AppHost
```

## Project Architecture

**Current State:** Minimal Aspire starter ready for microservices development.

```
┌─────────────────────────────────────────────────────────────┐
│                    HippoCamp Architecture                   │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  AppHost (HippoCamp.AppHost)                               │
│  ├── Program.cs: DistributedApplication.CreateBuilder()    │
│  ├── Orchestrates: [Ready for services]                    │
│  └── Dashboard: https://localhost:17134                    │
│                                                             │
│  Tests (HippoCamp.Tests)                                   │
│  ├── xUnit v3 with Microsoft Testing Platform             │
│  ├── FluentAssertions + Moq                               │
│  └── Aspire.Hosting.Testing (in-memory E2E)               │
│                                                             │
│  [Future Services - To Be Added]                          │
│  ├── Web Frontend (Blazor)                                │
│  ├── API Services                                         │
│  ├── Background Workers                                   │
│  └── Resources (SQL Server, Redis, etc.)                  │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

**Configuration:** Uses user secrets, appsettings.json, and environment variables following .NET configuration precedence.

## Development Patterns & Standards

### Code Conventions
- **Async everywhere:** Use `async`/`await` with `CancellationToken` parameters
- **Small functions:** Single-purpose, focused methods
- **Composition over inheritance:** Prefer dependency injection and composition
- **Immutability:** Don't mutate parameters passed to functions
- **Constants over magic strings:** Define string constants for repeated values
- **File-scoped namespaces:** Required for all new C# files (enforced by EditorConfig)

### Testing Principles
- **One logical assertion per test:** Keep tests focused and clear
- **Deterministic tests:** No random data, time dependencies, or external calls
- **Real objects over mocks:** Avoid over-mocking simple value objects
- **Tests as documentation:** Test names and structure should explain behavior
- **Use `ITestOutputHelper`** for test debugging output
- **Aspire.Hosting.Testing:** For integration tests with in-memory AppHost

### Example Test Structure
```csharp path=null start=null
public class UserServiceTests
{
    private readonly ITestOutputHelper _output;
    
    public UserServiceTests(ITestOutputHelper output) => _output = output;
    
    [Theory]
    [InlineData("valid@email.com", true)]
    [InlineData("invalid-email", false)]
    public void ValidateEmail_ShouldReturnExpectedResult(string email, bool expected)
    {
        // Act
        var result = UserService.ValidateEmail(email);
        
        // Assert
        result.Should().Be(expected);
    }
}
```

## Git Workflow (Strictly Enforced)

**⚠️ CRITICAL: Never commit directly to `develop` or `main` branches**

### Issue-Driven Development (Mandatory)
All work must be linked to GitHub issues:

```bash
# 1. Create or find issue
gh issue create --title "feat: add user authentication" --label enhancement

# 2. Create feature branch with issue number
git checkout develop && git pull
git checkout -b feature/42-user-authentication

# 3. Develop with conventional commits
git commit -m "feat(auth): add JWT service"
git commit -m "test(auth): add JWT service tests"

# 4. Update issue and create PR
gh issue comment 42 --body "✅ Implementation complete, opening PR"
git push -u origin feature/42-user-authentication
gh pr create --base develop --title "feat: add user authentication" --body "Closes #42"
```

### Branch Strategy
- `feature/#{issue}-description` → `develop` (squash merge)
- `hotfix/#{issue}-description` → `main` (squash merge)
- `develop` → `main` (merge commit, releases only)

### Conventional Commits (Required)
```bash
feat(scope): add new feature
fix(scope): resolve bug
docs: update documentation  
test: add or update tests
refactor: improve code structure
chore: update dependencies
```

### Commit Best Practices
**Prefer small, focused commits over large all-encompassing ones:**

```bash
# ✅ Good: Small, focused commits
git commit -m "feat(auth): add JWT token validation"
git commit -m "test(auth): add JWT validation tests"
git commit -m "docs(auth): update authentication README"

# ❌ Bad: Large, unfocused commit
git commit -m "feat: add complete authentication system with tests and docs"
```

**Benefits of small commits:**
- Easier code review and debugging
- Clearer git history and blame information
- Safer to revert specific changes
- Better CI/CD feedback loop
- Simplified merge conflict resolution

**Grouping Guidelines:**
- Group related file types (tests with tests, docs with docs)
- Separate feature implementation from configuration changes
- Keep refactoring separate from new features
- Commit formatting/style changes independently

## Troubleshooting

### Common Issues
```bash
# Port conflicts (Aspire uses many ports)
lsof -i :17134                   # Check if Aspire Dashboard port is in use
pkill -f "dotnet.*AppHost"       # Kill running AppHost processes

# Clear NuGet cache
dotnet nuget locals all --clear

# Reset Docker (if containers misbehave)
docker system prune -f

# Restore .NET workloads (if needed)
dotnet workload restore
```

### Build Issues
```bash
# Clean and rebuild everything
dotnet clean && dotnet restore && dotnet build

# Check for SDK version mismatches
cat global.json
dotnet --version

# Verify project references
dotnet list reference
```

## Dependency Management

### Automated Updates (Dependabot)
The repository uses Dependabot for automated dependency updates:

- **NuGet packages**: Updated weekly on Mondays
- **.NET SDK**: Updated weekly on Tuesdays  
- **GitHub Actions**: Updated weekly on Wednesdays

**Package Grouping** (to reduce PR noise):
- Microsoft packages (`Microsoft.*`, `System.*`) grouped together
- Aspire packages (`Aspire.*`) grouped together
- Testing packages (`xunit*`, `FluentAssertions*`, `Moq*`, etc.) grouped together

### Manual Dependency Management
```bash
# Check for outdated packages
dotnet list package --outdated

# Check for vulnerable packages
dotnet list package --vulnerable

# Update all packages to latest compatible versions
dotnet list package --outdated | grep ">" | cut -d">" -f2 | xargs -I {} dotnet add package {}

# Update specific package
dotnet add package Microsoft.AspNetCore.App --version 9.0.0

# Remove unused package references
dotnet list package --include-transitive | grep "Auto-referenced"
```

### Dependency Review Process
```bash
# Review dependabot PRs
gh pr list --label "dependencies" --state open

# Quick review of changes in a dependabot PR
gh pr diff <pr-number>

# Approve and merge dependabot PR (after validation)
gh pr review <pr-number> --approve
gh pr merge <pr-number> --squash
```

## Environment Configuration

### Development Environments
```bash
# Run in different environments
export ASPNETCORE_ENVIRONMENT=Development
export DOTNET_ENVIRONMENT=Development

# Run AppHost with specific environment
dotnet run --project src/HippoCamp.AppHost -e ASPNETCORE_ENVIRONMENT=Staging
```

### Aspire Dashboard Environment Variables
- `ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL`: HTTPS OTLP endpoint
- `ASPIRE_RESOURCE_SERVICE_ENDPOINT_URL`: Resource service endpoint
- Dashboard auto-configures these when running via AppHost

## Project Structure Key Points

- **`src/HippoCamp.AppHost/`**: Aspire orchestration host (main entry point)
- **`tests/HippoCamp.Tests/`**: Integration and unit tests with Aspire.Hosting.Testing
- **`.serena/`**: AI assistant configuration and memories
- **`.github/workflows/`**: CI/CD with strict Git-flow enforcement
- **`Directory.Build.props`**: Global project settings (nullable enabled, implicit usings)
- **`global.json`**: .NET 9.0.304 SDK version pinning

## Sources & References

This guide was created using verified information from:

- .NET Aspire Documentation (Microsoft Learn, accessed 2025-01-03)
- .NET 9 Documentation (Microsoft Docs, accessed 2025-01-03) 
- Warp Terminal Documentation (warp.dev, accessed 2025-01-03)
- Project README.md and configuration files

**Version Information:**
- .NET SDK: 9.0.304 (specified in global.json)
- Aspire: 9.4.2 (specified in project files)
- xUnit: v3 with Microsoft Testing Platform
- FluentAssertions: 8.6.0, Moq: 4.20.72
