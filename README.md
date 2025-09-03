# HippoCamp

![CodeRabbit Pull Request Reviews](https://img.shields.io/coderabbit/prs/github/mhagrelius/HippoCamp?utm_source=oss&utm_medium=github&utm_campaign=mhagrelius%2FHippoCamp&labelColor=171717&color=FF570A&link=https%3A%2F%2Fcoderabbit.ai&label=CodeRabbit+Reviews)

A cloud-native .NET 9 application built with **Microsoft .NET Aspire** for orchestrating distributed services. This project serves as a foundation for building modern microservices architectures with integrated AI-assisted development workflows.

## Quick Start

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (9.0.304 or later)
- [Aspire CLI](https://learn.microsoft.com/dotnet/aspire/fundamentals/setup-tooling) (`dotnet workload install aspire`)
- [GitHub CLI](https://cli.github.com/) (for repository management)

### Run the Application
```bash
# Clone and setup
git clone https://github.com/mhagrelius/HippoCamp.git
cd HippoCamp
dotnet restore

# Run with Aspire
aspire run

# Alternative: Run AppHost directly
dotnet run --project src/HippoCamp.AppHost

# Run tests
dotnet test
```

## Tech Stack

- **.NET 9.0** - Latest framework with C# 13
- **Microsoft Aspire 9.4.2** - Cloud-native orchestration
- **xUnit v3** - Modern testing with Microsoft Testing Platform
- **FluentAssertions + Moq** - Enhanced testing capabilities
- **Entity Framework Core** - Ready for data access
- **Blazor** - Ready for web UI

## Development Workflow

### Git-Flow Branch Strategy

We follow **Git-flow** with **issue-linked branches** for better traceability:

```bash
# 🎯 Start new feature (linked to GitHub issue)
# Note: Features include new functionality AND non-urgent bug fixes
git checkout develop && git pull
git checkout -b feature/#{issue-number}-{short-description}

# 🚨 Emergency hotfix (linked to GitHub issue)
# Note: Only for urgent production fixes that can't wait for next release
git checkout main && git pull
git checkout -b hotfix/#{issue-number}-{short-description}
```

#### Branch Naming Examples
```bash
feature/15-user-authentication      # New feature
feature/23-fix-login-validation     # Non-urgent bug fix
hotfix/42-critical-security-patch   # Emergency production fix
```

**Branch Targeting**:
- `feature/*` → target `develop` branch
- `hotfix/*` → target `main` branch (then sync back to develop)

### Commit Conventions

We enforce **Conventional Commits** for clear history:

```bash
# Format: type(scope): description
feat: add user authentication service
fix: resolve login validation error
docs: update API documentation
test: add integration tests for auth
refactor: simplify user service logic
chore: update dependencies
```

#### Common Types
- `feat`: New features
- `fix`: Bug fixes
- `docs`: Documentation changes
- `test`: Adding or updating tests
- `refactor`: Code refactoring
- `chore`: Maintenance tasks
- `ci`: CI/CD changes

### Pull Request Process

1. **Create feature branch** with issue number
2. **Develop and test** your changes
3. **Commit with conventional format**
4. **Push and create PR**:
   ```bash
   git push -u origin feature/#{issue-number}-{description}
   gh pr create --base develop --title "feat: your feature description" --body "Closes ##{issue-number}"
   ```
5. **Wait for CI validation** (builds, tests, linting)
6. **Request review** from team members
7. **Merge using squash** (feature → develop) or **merge commit** (develop → main)

### Automated Enforcement

✅ **GitHub Actions validate**:
- Conventional commit format
- Git-flow branch naming
- Build compilation
- Test execution
- No direct commits to main

## Code Standards

### Engineering Principles
- **Async/await**: Preferred for I/O operations with cancellation tokens
- **Small functions**: Single-purpose, focused functions
- **Composition over inheritance**: Prefer composition patterns
- **Immutability**: Prefer immutable data structures
- **Constants**: Use constants instead of magic strings
- **Limited exceptions**: Avoid exceptions for control flow

### Style Guide
- **File-scoped namespaces**: Required for all new files
- **Nullable reference types**: Enabled project-wide
- **var usage**: Required for built-in and apparent types
- **Private fields**: Use `_camelCase` naming
- **EditorConfig**: Enforced automatically

### Testing Standards
- **xUnit with FluentAssertions**: Core testing stack
- **Theory/InlineData**: For data-driven tests
- **One logical assertion per test**: Keep tests focused
- **Deterministic tests**: No random data or timing dependencies
- **Test file organization**: Break up by functionality
- **Real objects over mocks**: Avoid over-mocking simple classes
- **ITestOutputHelper**: For test debugging output

## Project Structure

```
HippoCamp/
├── src/
│   └── HippoCamp.AppHost/          # 🎯 Aspire orchestration host
├── tests/
│   └── HippoCamp.Tests/            # 🧪 Integration and unit tests
├── docs/
│   └── project-initialization-summary.md  # 📚 Detailed setup guide
├── .serena/                        # 🤖 AI assistant configuration
├── .github/
│   ├── workflows/                  # 🔄 CI/CD automation
│   └── pull_request_template.md    # 📝 PR guidelines
└── [Configuration files]
```

## Commands Reference

### Development
```bash
dotnet restore              # Restore NuGet packages
dotnet build               # Build entire solution
dotnet test                # Run all tests (Microsoft Testing Platform)
dotnet test -v detailed    # Verbose test output
aspire run                 # Run Aspire application stack
```

### Git Workflow
```bash
# Start new work (features and non-urgent fixes)
git checkout develop && git pull
git checkout -b feature/#{issue}-{name}

# Emergency hotfix
git checkout main && git pull
git checkout -b hotfix/#{issue}-{name}

# Daily workflow
git add .
git commit -m "feat: implement feature"  # or "fix: resolve bug"
git push -u origin feature/#{issue}-{name}

# Create PR (target develop for features, main for hotfixes)
gh pr create --base develop --title "feat: feature name" --body "Closes ##{issue}"
```

## Documentation

- **📋 [Project Initialization Summary](docs/project-initialization-summary.md)** - Complete setup and architecture guide
- **🔧 [EditorConfig](.editorconfig)** - Code style configuration
- **🚀 [GitHub Actions](.github/workflows/)** - Automated validation workflows

## Contributing

1. **Check existing issues** or create a new one
2. **Follow Git-flow**: Create feature branch with issue number
3. **Use conventional commits**: Clear, standardized commit messages
4. **Write tests**: Maintain high code coverage
5. **Update documentation**: Keep docs current with changes
6. **Request reviews**: All changes need approval

For detailed setup and architecture information, see [Project Initialization Summary](docs/project-initialization-summary.md).

---

**Status**: ✅ Fully initialized and ready for development with modern .NET practices and AI-assisted workflows.
