# Suggested Commands for HippoCamp Development

## Essential Development Commands

### Setup and Restoration
```bash
# Restore NuGet packages
dotnet restore

# Build entire solution
dotnet build

# Clean build artifacts
dotnet clean
```

### Running the Application
```bash
# Run the Aspire application (preferred method)
aspire run

# Alternative: Run AppHost directly
dotnet run --project src/HippoCamp.AppHost

# Run in watch mode (auto-restart on changes)
dotnet watch --project src/HippoCamp.AppHost
```

### Testing
```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test -v detailed

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests/HippoCamp.Tests
```

### Aspire-Specific Commands
```bash
# Create new Aspire projects
aspire new aspire-starter --name MyProject
aspire new aspire-apphost --name MyHost

# Add integrations to Aspire AppHost
aspire add

# Check Aspire CLI configuration
aspire config

# View available templates
dotnet new list aspire
```

### Package Management
```bash
# Add NuGet package to specific project
dotnet add [PROJECT] package [PACKAGE_NAME]

# Add project reference
dotnet add [PROJECT] reference [REFERENCE_PROJECT]

# List package references
dotnet list [PROJECT] package
```

### Git Flow Commands
```bash
# Start new feature
git checkout develop
git pull
git checkout -b feature/my-feature-name

# Commit with conventional commits
git commit -m "feat: add new feature"
git commit -m "fix(auth): resolve login issue"
git commit -m "docs: update README"

# Push feature branch
git push -u origin feature/my-feature-name
```

### Utility Commands (macOS)
```bash
# File operations
ls -la              # List files with details
find . -name "*.cs" # Find C# files
grep -r "pattern"   # Search for text patterns
code .              # Open in VS Code

# Process management
ps aux | grep dotnet  # Find running .NET processes
killall dotnet       # Stop all .NET processes

# System information
uname -a            # System info
which dotnet        # .NET installation path
echo $PATH          # View PATH variable
```

### Development Tools
```bash
# Format code (if using dotnet-format)
dotnet format

# Security scan
dotnet list package --vulnerable

# Outdated packages
dotnet list package --outdated
```

## Quick Reference
- **Primary run command**: `aspire run`
- **Primary test command**: `dotnet test`
- **Build command**: `dotnet build`
- **Branch workflow**: feature/bugfix branches → develop → main