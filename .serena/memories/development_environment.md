# System and Development Environment

## Operating System: macOS (Darwin)

### System Utilities
- **Shell**: zsh (version 5.9)
- **Package manager**: Homebrew (typical for macOS)
- **File system**: Case-sensitive APFS
- **Line endings**: Unix-style (LF)

### Essential macOS Commands
```bash
# File operations
ls -la                   # List files with details and hidden files
find . -name "*.cs"      # Find files by pattern
grep -r "search"         # Recursive text search
open .                   # Open current directory in Finder
code .                   # Open in VS Code (if installed)

# Process management
ps aux | grep dotnet     # Find .NET processes
kill -9 PID             # Force kill process
killall dotnet          # Kill all dotnet processes
lsof -i :5000           # Find what's using port 5000

# System information
uname -a                 # System information
which dotnet            # Find dotnet installation
echo $PATH              # View PATH variable
env                     # View environment variables
pwd                     # Current directory
whoami                  # Current user
```

### File Permissions (macOS)
```bash
chmod +x script.sh      # Make script executable
chown user:group file   # Change file ownership
ls -l                   # View file permissions
```

## Development Tools

### .NET Tools
- **SDK**: 9.0.304 (installed at /usr/local/share/dotnet/)
- **CLI**: dotnet command with full SDK
- **Runtime**: .NET 9.0

### Aspire Tools
- **CLI**: 9.4.2 (native AOT, installed at ~/.aspire/bin/aspire)
- **Templates**: 9.4.2 via Aspire.ProjectTemplates
- **Dashboard**: Built-in with app host

### Container Runtime (Required for Aspire)
- **Docker Desktop**: Recommended for macOS
- **Alternative**: Podman (set ASPIRE_CONTAINER_RUNTIME=podman)
- **Health Check**: `docker --version` or `podman --version`

### Git Configuration
- **Version Control**: Git with GitHub integration
- **Branch**: Currently on `develop` (default)
- **Remotes**: origin (GitHub)

## IDE and Editor Support

### Recommended IDEs
- **Visual Studio Code**: With C# Dev Kit extension
- **JetBrains Rider**: With .NET Aspire plugin
- **Visual Studio for Mac**: With .NET workloads

### EditorConfig Integration
- Format on save enabled
- Consistent indentation (4 spaces for C#)
- UTF-8 encoding
- LF line endings
- Trailing whitespace removal

## Environment Variables and Paths

### Important Paths
- **Project root**: /Users/matthew/Code/HippoCamp
- **Home directory**: /Users/matthew
- **.NET installation**: /usr/local/share/dotnet/
- **NuGet packages**: ~/.nuget/packages/
- **Aspire CLI**: ~/.aspire/bin/aspire

### Required Environment Setup
- **PATH**: Includes .NET SDK and Aspire CLI
- **DOTNET_ROOT**: Points to .NET installation
- **Optional**: ASPIRE_CONTAINER_RUNTIME (if using Podman)

## Security Considerations

### Development Certificates
- HTTPS development certificate may need manual trust
- Use: `dotnet dev-certs https --trust`
- Certificate location: User keychain (macOS)

### Secrets Management
- **User secrets**: Configured per project
- **Command**: `dotnet user-secrets set "key" "value" --project src/HippoCamp.AppHost`
- **Storage**: ~/.microsoft/usersecrets/[UserSecretsId]/

### File Permissions
- Source code: Standard user permissions
- Scripts: May need execute permissions (chmod +x)
- Certificates: Keychain access required

This environment setup ensures optimal development experience on macOS with .NET 9 and Aspire.