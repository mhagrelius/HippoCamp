# HippoCamp Project Overview

## Purpose
HippoCamp is a .NET 9 cloud-native application built with Microsoft .NET Aspire for orchestrating distributed services. The project is designed to leverage modern .NET development practices including:

- Microservices architecture with Aspire orchestration
- Cloud-native deployment patterns
- Integration with Serena AI assistant for enhanced development workflow
- Modern testing and development practices

## Current State
- **Status**: Newly initialized project with foundational structure
- **Primary Goal**: Provide a robust foundation for building distributed .NET applications
- **AI Integration**: Configured for Serena AI assistant to enhance development productivity

## Tech Stack
- **.NET 9.0**: Latest LTS version of .NET
- **Aspire 9.4.2**: Microsoft's cloud-native application platform
- **Entity Framework Core**: Ready for integration (following user preferences)
- **Blazor**: Ready for integration (following user preferences)
- **xUnit + FluentAssertions + Moq**: Testing framework stack
- **Git Flow**: Version control strategy with develop/main branch model

## Project Structure
```
HippoCamp/
├── src/
│   └── HippoCamp.AppHost/          # Aspire orchestration host
├── tests/
│   └── HippoCamp.Tests/            # Integration tests
├── .serena/                        # Serena AI configuration
├── .github/                        # GitHub Actions and templates
├── global.json                     # .NET SDK version pinning
├── Directory.Build.props           # Common MSBuild properties
├── .editorconfig                   # Code style configuration
└── HippoCamp.sln                   # Solution file
```

## Key Features
- Aspire-native project structure
- Comprehensive testing setup with modern libraries
- GitHub Actions for PR validation
- Conventional commit enforcement
- Git-flow branch strategy enforcement
- Code style consistency via EditorConfig