---
created: 2025-09-04T22:32:50Z
last_updated: 2025-09-05T01:08:54Z
version: 1.1
author: Claude Code PM System
---

# Technical Context

## Core Technology Stack

### .NET Platform
- **.NET Version**: 9.0 (Latest LTS)
- **C# Language**: 13.0 (Latest features enabled)
- **SDK Version**: 9.0.304 (pinned in global.json)
- **Roll Forward Policy**: latestFeature
- **Nullable Reference Types**: Enabled project-wide
- **Implicit Usings**: Enabled for cleaner code

### Cloud-Native Framework
- **Microsoft Aspire**: 9.4.2
  - `Aspire.AppHost.Sdk` - Application host orchestration
  - `Aspire.Hosting.AppHost` - Core hosting capabilities
  - `Aspire.Hosting.Testing` - Integration testing support
- **User Secrets**: Configured for secure local development

### Testing Framework
- **xUnit v3**: 2.0.3 (Next-generation testing)
- **Microsoft Testing Platform**: 1.6.3 (Modern test runner)
- **FluentAssertions**: 8.6.0 (Expressive assertions)
- **Moq**: 4.20.72 (Mocking framework)
- **Coverlet Collector**: 6.0.4 (Code coverage)
- **Microsoft.NET.Test.Sdk**: 17.10.0 (Test SDK)
- **xUnit Visual Studio Runner**: 3.1.0 (IDE integration)

### Development Tools & Dependencies

#### Build & Compilation
- **MSBuild**: Integrated with .NET SDK
- **Global Usings**: Pre-configured for common namespaces
  - `System.Net`
  - `Microsoft.Extensions.DependencyInjection`
  - `Aspire.Hosting.ApplicationModel`
  - `Aspire.Hosting.Testing`
  - `Xunit`

#### Code Quality
- **EditorConfig**: Comprehensive style enforcement
- **Nullable Context**: Enabled for null safety
- **Implicit Usings**: Reduces boilerplate code
- **File-Scoped Namespaces**: Modern C# style

## Infrastructure Requirements

### Container Runtime
- **Docker Desktop** or **Podman** required
- Used by Aspire for service orchestration
- Local development environment containerization

### Development Prerequisites
- **.NET 9.0 SDK**: 9.0.304 or later
- **Aspire CLI**: Global tool installation
- **GitHub CLI**: For repository management
- **Container Runtime**: Docker Desktop or Podman

## AI-Assisted Development Stack

### Claude Code Integration
- **Project Management System**: Custom PM scripts and agents
- **Context System**: Comprehensive project documentation
- **Specialized Agents**:
  - Code analysis and bug detection
  - File content summarization
  - Test execution and analysis
  - Parallel workflow coordination

### Serena Memory System
- **Persistent Knowledge**: AI assistant memory storage
- **Project Intelligence**: Accumulated development insights
- **Context Preservation**: Cross-session project understanding

## Configuration Management

### Project Configuration
- **Solution File**: HippoCamp.sln (Visual Studio solution)
- **Directory.Build.props**: Shared MSBuild properties
- **global.json**: SDK version control and rollforward policy
- **User Secrets**: Secure configuration for local development

### Environment Configuration
- **EditorConfig**: Cross-IDE code style consistency
- **GitIgnore**: Comprehensive exclusion rules
- **GitHub Workflows**: CI/CD automation configuration

## Package Management

### NuGet Packages
All packages use explicit versioning for reproducible builds:

**Core Framework**:
- Aspire.Hosting.AppHost (9.4.2)
- Aspire.AppHost.Sdk (9.4.2)

**Testing Suite**:
- xunit.v3 (2.0.3)
- FluentAssertions (8.6.0)
- Moq (4.20.72)
- Microsoft.Testing.Platform.MSBuild (1.6.3)

**Development Tools**:
- coverlet.collector (6.0.4)
- Microsoft.NET.Test.Sdk (17.10.0)
- xunit.runner.visualstudio (3.1.0)

### Package Strategy
- **Explicit versioning** for all external dependencies
- **Central package management** via Directory.Build.props
- **Security-first** approach with regular updates
- **Minimal dependencies** to reduce attack surface

## Development Environment

### Local Development
- **Hot Reload**: Enabled for rapid development cycles
- **Aspire Dashboard**: Local service monitoring and debugging
- **Container Orchestration**: Automatic service startup and configuration
- **Secret Management**: User secrets for local development

### Build & Test Pipeline
- **Continuous Integration**: GitHub Actions automation
- **Automated Testing**: Full test suite on every commit
- **Code Quality**: Automated style and quality checks
- **Security Scanning**: Dependency vulnerability analysis

## Future Technology Considerations

### Planned Additions
- **Entity Framework Core**: Data access layer implementation
- **Authentication/Authorization**: Identity management system
- **API Framework**: RESTful service endpoints
- **Monitoring & Observability**: Application Performance Monitoring (APM)

### Scalability Considerations
- **Microservices Architecture**: Aspire-native service decomposition
- **Cloud Deployment**: Azure Container Apps or Kubernetes ready
- **Database Strategy**: Multi-database support via EF Core
- **Caching Strategy**: Distributed caching for performance

This technical foundation provides a modern, cloud-native development platform with comprehensive testing, AI assistance, and scalable architecture patterns.

## Update History
- 2025-09-05T01:08:54Z: Confirmed all dependencies successfully implemented in project files - no version changes needed as context was already accurate.