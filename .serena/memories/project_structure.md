# Detailed Project Structure

## Solution Architecture
The HippoCamp solution follows a clean, Aspire-native architecture:

```
HippoCamp/                          # Root solution directory
├── src/
│   └── HippoCamp.AppHost/          # Aspire orchestration host
├── tests/
│   └── HippoCamp.Tests/            # Integration and unit tests
├── .serena/                        # Serena AI configuration
├── .github/                        # GitHub Actions and templates
├── docs/ (planned)                 # Project documentation
└── [Root configuration files]
```

## Project Details

### HippoCamp.AppHost
- **Type**: Aspire App Host (orchestration)
- **Framework**: .NET 9.0
- **Purpose**: Central orchestration point for distributed services
- **Key Files**:
  - `AppHost.cs`: Main orchestration configuration
  - `appsettings.json`: Configuration settings
  - `appsettings.Development.json`: Development-specific settings
- **Dependencies**: Aspire.Hosting.AppHost 9.4.2
- **User Secrets ID**: 50c6e828-6a1a-496f-80d4-2300a67844ff

### HippoCamp.Tests
- **Type**: Test project (xUnit)
- **Framework**: .NET 9.0
- **Purpose**: Integration and unit testing for the application
- **Key Libraries**:
  - xUnit v3 (2.0.3) - Test framework
  - FluentAssertions (8.6.0) - Assertion library
  - Moq (4.20.72) - Mocking framework
  - Aspire.Hosting.Testing (9.4.2) - Aspire testing utilities
- **Global Usings**: Includes common testing namespaces
- **Sample**: Integration test template for Aspire apps

## Configuration Files

### Root Level
- **global.json**: Pins .NET SDK to 9.0.304 with latestFeature rollForward
- **Directory.Build.props**: Sets common properties (net9.0, ImplicitUsings, Nullable)
- **.editorconfig**: Based on official ASP.NET Core repository standards
- **.gitignore**: Comprehensive .NET gitignore with .serena/ explicitly included
- **HippoCamp.sln**: Visual Studio solution file

### GitHub Configuration
- **.github/workflows/pr-validation.yml**: Automated PR validation
- **.github/PULL_REQUEST_TEMPLATE.md**: Standard PR template with git-flow guidance

### Serena Configuration
- **.serena/project.yml**: Configures project as C# for Serena AI assistant

## Dependencies and Versions

### Development Tools
- .NET SDK: 9.0.304
- Aspire CLI: 9.4.2 (native AOT version)
- Aspire Templates: 9.4.2

### Package Versions
- Aspire.Hosting.AppHost: 9.4.2
- Aspire.Hosting.Testing: 9.4.2
- xunit.v3: 2.0.3
- FluentAssertions: 8.6.0
- Moq: 4.20.72

## Future Structure (Planned)
As the project grows, the typical structure would expand to:

```
src/
├── HippoCamp.AppHost/              # Orchestration
├── HippoCamp.Api/                  # Web API project
├── HippoCamp.Web/                  # Blazor web application
├── HippoCamp.Domain/               # Domain models and business logic
├── HippoCamp.Infrastructure/       # Data access and external services
└── HippoCamp.ServiceDefaults/      # Shared Aspire service configuration

tests/
├── HippoCamp.Tests/                # Integration tests
├── HippoCamp.Api.Tests/            # API unit tests
├── HippoCamp.Domain.Tests/         # Domain unit tests
└── HippoCamp.Infrastructure.Tests/ # Infrastructure unit tests
```