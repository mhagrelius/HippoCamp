# HippoCamp

![CodeRabbit Pull Request Reviews](https://img.shields.io/coderabbit/prs/github/mhagrelius/HippoCamp?utm_source=oss&utm_medium=github&utm_campaign=mhagrelius%2FHippoCamp&labelColor=171717&color=FF570A&link=https%3A%2F%2Fcoderabbit.ai&label=CodeRabbit+Reviews)

A cloud-native .NET 9 application built with **Microsoft .NET Aspire** for orchestrating distributed services. This project serves as a foundation for building modern microservices architectures with integrated AI-assisted development workflows.

## Quick Start

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (9.0.304 or later)
- [Aspire CLI](https://learn.microsoft.com/dotnet/aspire/fundamentals/setup-tooling) (`dotnet tool install -g Aspire.Cli`)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) or [Podman](https://podman.io/) (container runtime)
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

## Development Workflow

### Issue-Driven Development Process

**ALL work must be tied to GitHub issues** for proper tracking and context:

```bash
# 📋 Step 1: Create or assign GitHub issue
gh issue create --title "feat: add user authentication" --body "Description of work" --label enhancement
# OR: gh issue list --assignee @me

# 🌿 Step 2: Create issue-linked branch
git checkout develop && git pull
git checkout -b feature/#{issue-number}-{short-description}

# 💻 Step 3: Develop and commit with conventional format
git commit -m "feat: implement user authentication service"

# 💬 Step 4: Update issue with progress
gh issue comment #{issue-number} --body "✅ Implementation complete, opening PR"

# 🔄 Step 5: Create PR linking back to issue
git push -u origin feature/#{issue-number}-{description}
gh pr create --base develop --title "feat: implement feature" --body "Closes ##{issue-number}"
```

### Git-Flow Branch Strategy

We follow **standard Git-flow** with **mandatory issue linking**:

```bash
# 🎯 New features and non-urgent fixes
git checkout develop && git pull
git checkout -b feature/#{issue-number}-{short-description}

# 🚨 Emergency production hotfixes
git checkout main && git pull
git checkout -b hotfix/#{issue-number}-{short-description}

# 📦 Release preparation (when ready)
git checkout develop && git pull
git checkout -b release/v{major.minor.patch}
```

#### Branch Naming Examples
```bash
feature/15-user-authentication      # New feature (issue #15)
feature/23-fix-login-validation     # Non-urgent fix (issue #23)
hotfix/42-critical-security-patch   # Emergency fix (issue #42)
release/v1.0.0                     # Release branch
```

**Branch Targeting & Merge Strategy**:
- `feature/*` → `develop` (squash merge)
- `hotfix/*` → `main` (squash merge) + sync to `develop`
- `develop` → `main` (merge commit for releases)

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

### Versioning & Release Tagging

We use **Semantic Versioning (SemVer)** with Git tags:

```bash
# Release workflow (develop → main)
gh pr create --base main --head develop --title "release: v1.2.3" --body "Release version 1.2.3"
# After merge to main:
git checkout main && git pull
git tag -a v1.2.3 -m "Release v1.2.3"
git push origin v1.2.3

# Hotfix workflow (hotfix → main)
gh pr create --base main --title "hotfix: critical security fix" --body "Closes ##{issue}"
# After merge:
git tag -a v1.2.4 -m "Hotfix v1.2.4 - security patch"
git push origin v1.2.4
```

#### Versioning Rules
- **MAJOR** (v2.0.0): Breaking changes, API changes
- **MINOR** (v1.1.0): New features, backward-compatible
- **PATCH** (v1.0.1): Bug fixes, security patches
- **Pre-release** (v1.0.0-beta.1): Development versions

### Complete Development Workflow
```bash
# 📋 Always start with an issue
gh issue create --title "feat: add authentication" --label enhancement
# Note the issue number (e.g., #25)

# 🌿 Create linked feature branch
git checkout develop && git pull
git checkout -b feature/25-user-authentication

# 💻 Develop with conventional commits
git add .
git commit -m "feat: add authentication service"
git commit -m "test: add authentication tests"

# 💬 Update issue progress
gh issue comment 25 --body "✅ Authentication service implemented, tests added"

# 🔄 Create PR with issue link
git push -u origin feature/25-user-authentication
gh pr create --base develop --title "feat: add user authentication" --body "Closes #25

Implements user authentication service with:
- JWT token generation
- Password hashing
- Session management
- Full test coverage"

# 🎯 After review approval: squash merge to develop
# 📦 When ready for release: create release PR (develop → main) and tag
```

## Documentation

- **🔧 [EditorConfig](.editorconfig)** - Code style configuration
- **🚀 [GitHub Actions](.github/workflows/)** - Automated validation workflows
- **📝 [PR Template](.github/PULL_REQUEST_TEMPLATE.md)** - Pull request guidelines

## Contributing

### 📋 **MANDATORY**: Issue-First Development

**All work MUST start with a GitHub issue** - no exceptions for developers or AI agents:

#### Complete Issue-Driven Workflow:
```bash
# 1️⃣ CREATE OR ASSIGN ISSUE
gh issue create --title "feat: add user service" --body "Detailed description" --label enhancement
# Note the issue number (e.g., #47)

# 2️⃣ CREATE ISSUE-LINKED BRANCH  
git checkout develop && git pull
git checkout -b feature/47-user-service

# 3️⃣ DEVELOP WITH PROGRESS UPDATES
git commit -m "feat: scaffold user service structure"
gh issue comment 47 --body "🔧 Service scaffolding complete, working on business logic"

git commit -m "feat: implement user authentication logic" 
gh issue comment 47 --body "✅ Authentication implemented, adding tests"

git commit -m "test: add comprehensive user service tests"
gh issue comment 47 --body "🧪 Testing complete, ready for review"

# 4️⃣ CREATE LINKED PULL REQUEST
git push -u origin feature/47-user-service
gh pr create --base develop --title "feat: add user authentication service" --body "Closes #47

Implements user authentication with:
- JWT token management  
- Password hashing with BCrypt
- Role-based authorization
- Comprehensive test coverage
- Integration with Aspire configuration"

# 5️⃣ ISSUE AUTO-CLOSES ON MERGE
# When PR is squash-merged to develop, issue #47 automatically closes
```

#### Enforcement Rules:
✅ **Branch naming**: Must include issue number (`feature/#{issue}-description`)  
✅ **PR linking**: Must reference issue (`Closes ##{issue}`)  
✅ **Progress updates**: Comment on issues throughout development  
✅ **Automated validation**: GitHub Actions enforce these patterns

### Development Process

1. **Check existing issues** or create a new one
2. **Follow Git-flow**: Create issue-linked feature branch 
3. **Use conventional commits**: Clear, standardized commit messages
4. **Write tests**: Maintain high code coverage
5. **Update issue progress**: Keep stakeholders informed
6. **Create linked PR**: Reference issue in PR description
7. **Request reviews**: All changes need approval

---

**Status**: ✅ Fully initialized and ready for development with modern .NET practices and AI-assisted workflows.
