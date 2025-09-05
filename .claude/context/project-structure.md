---
created: 2025-09-04T22:32:50Z
last_updated: 2025-09-05T01:08:54Z
version: 2.0
author: Claude Code PM System
---

# Project Structure

## Solution Architecture

HippoCamp follows a clean, Aspire-native architecture optimized for cloud-native development:

```
HippoCamp/                          # Root solution directory
├── src/
│   └── HippoCamp.AppHost/          # 🎯 Aspire orchestration host
├── tests/
│   └── HippoCamp.Tests/            # 🧪 Integration and unit tests
├── .claude/                        # 🤖 AI assistance system
│   ├── agents/                     # Specialized AI agents
│   ├── context/                    # Project context documentation
│   └── scripts/pm/                 # Project management utilities
├── .serena/                        # 🧠 AI assistant memory system
│   └── memories/                   # Persistent knowledge base
├── .github/
│   ├── workflows/                  # 🔄 CI/CD automation
│   └── pull_request_template.md    # 📝 PR guidelines
└── [Configuration files]
```

## Core Directories

### `/src` - Source Code
- **HippoCamp.AppHost/** - ✅ **Primary Aspire application host (IMPLEMENTED)**
  - `HippoCamp.AppHost.csproj` - Complete project file with Aspire dependencies
  - `AppHost.cs` - Main application host implementation
  - `Properties/launchSettings.json` - Development configuration
  - `appsettings.json` - Application settings
  - Orchestrates distributed services through Aspire
  - Entry point for cloud-native deployment

### `/tests` - Test Suite  
- **HippoCamp.Tests/** - ✅ **Comprehensive test coverage (IMPLEMENTED)**
  - `HippoCamp.Tests.csproj` - Complete test project with xUnit v3
  - `IntegrationTest1.cs` - Example integration test implementation
  - xUnit v3 with Microsoft Testing Platform configured
  - FluentAssertions and Moq dependencies included
  - Ready for comprehensive test development

### `/.claude` - AI Assistance System
- **agents/** - Specialized AI agents for development tasks
  - `code-analyzer.md` - Code analysis and bug detection
  - `file-analyzer.md` - File content analysis and summarization
  - `test-runner.md` - Test execution and analysis
  - `parallel-worker.md` - Parallel workflow coordination
- **context/** - Project context documentation (this directory)
- **scripts/pm/** - Project management automation
  - `init.sh` - System initialization
  - `validate.sh` - Validation utilities
  - `search.sh` - Project search capabilities
  - `epic-*.sh` - Epic management tools

### `/.serena` - AI Memory System
- **memories/** - Persistent knowledge base
  - `project_structure.md` - Detailed structure documentation
  - `project_overview.md` - High-level project understanding
  - `development_environment.md` - Environment setup details
  - `git_flow_governance.md` - Git workflow specifications
  - `code_style_conventions.md` - Coding standards and practices

## Configuration Files

### Solution Level
- **HippoCamp.sln** - ✅ **Complete Visual Studio solution file**
- **Directory.Build.props** - ✅ **MSBuild properties for all projects**
- **global.json** - ✅ **.NET SDK version pinning (9.0.304)**
- **commitlint.config.mjs** - ✅ **Conventional commit validation**

### Development Environment  
- **.editorconfig** - ✅ **Complete code style enforcement across IDEs**
- **.gitignore** - ✅ **Comprehensive version control exclusions**
- **README.md** - ✅ **Enhanced project documentation with setup guides**
- **WARP.md** - ✅ **WARP terminal development guide**

### GitHub Integration
- **.github/workflows/pr-validation.yml** - ✅ **Enhanced CI/CD automation**
- **.github/PULL_REQUEST_TEMPLATE.md** - ✅ **Standardized PR process**  
- **.github/dependabot.yml** - ✅ **Automated dependency updates**

## Naming Conventions

### File Naming
- **C# files**: PascalCase (e.g., `UserService.cs`)
- **Project files**: PascalCase with `.csproj` extension
- **Test files**: PascalCase with descriptive suffixes (e.g., `UserServiceTests.cs`)
- **Documentation**: kebab-case with `.md` extension

### Directory Structure
- **Source projects**: Under `/src` with descriptive names
- **Test projects**: Mirror source structure under `/tests`
- **Shared libraries**: Organized by functional domain
- **Configuration**: Root level for solution-wide configs

## Project Dependencies

### Internal Dependencies
- Projects reference each other through standard .NET project references
- Shared abstractions defined in dedicated projects
- Clean architecture boundaries between layers

### External Dependencies
- **Microsoft.NET.Aspire** - Cloud-native orchestration
- **xUnit** - Testing framework
- **FluentAssertions** - Expressive test assertions  
- **Moq** - Mocking framework for unit tests
- **Entity Framework Core** - Data access layer (configured)

## Module Organization

### Current Modules
1. **AppHost** - Service orchestration and configuration
2. **Tests** - Comprehensive test coverage

### Planned Modules (Future Development)
- **Core/Domain** - Business logic and domain models
- **Infrastructure** - External service integrations
- **API** - Web API endpoints and controllers
- **Frontend** - User interface components (if applicable)

## Build and Deployment Structure

### Build Artifacts
- Compiled to `/bin` directories per project
- NuGet packages restored to global cache
- Aspire deployment manifests generated during build

### Development Environment
- **Local development**: Aspire orchestrates services locally
- **Container support**: Docker Desktop or Podman required
- **Hot reload**: Enabled for rapid development cycles

This structure supports scalable development with clear separation of concerns, comprehensive testing, and AI-assisted development workflows.

## Update History
- 2025-09-05T01:08:54Z: MAJOR UPDATE - Complete .NET 9 Aspire implementation deployed. All project files created, solution structure implemented, test framework configured. Project fully ready for development.
- 2025-09-05T01:03:14Z: Corrected src/ and tests/ directories status - directories exist with build artifacts but .csproj files not created yet. Added warnings about current implementation status.