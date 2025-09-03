# Task Completion Guidelines

## When a Development Task is Completed

Follow these steps in order after completing any coding task:

### 1. Code Quality Verification
```bash
# Build to ensure compilation
dotnet build

# Run all tests to ensure nothing is broken
dotnet test

# Optional: Check for outdated or vulnerable packages
dotnet list package --outdated
dotnet list package --vulnerable
```

### 2. Git Flow and Commit Process

#### Before Committing
- Ensure you're on the correct feature branch (never commit directly to `develop` or `main`)
- Stage only the files related to your task
- Use conventional commit format

#### Commit Commands
```bash
# Stage specific files
git add src/path/to/changed/files

# Commit with conventional format
git commit -m "feat: add new feature description"
git commit -m "fix: resolve specific bug description"
git commit -m "docs: update documentation"
git commit -m "test: add tests for feature"
git commit -m "refactor: improve code structure"
git commit -m "chore: update dependencies"

# Push to feature branch
git push origin feature/branch-name
```

### 3. Pull Request Process

#### Create PR
```bash
# Using GitHub CLI (preferred)
gh pr create --base develop --title "feat: your feature title" --body "Description of changes"

# Or use GitHub web interface
```

#### PR Requirements
- **Target**: Always target `develop` branch (not `main`)
- **Title**: Use conventional commit format
- **Merge strategy**: Use "Squash and merge" for feature branches
- **Review**: Ensure all CI checks pass
- **Testing**: Verify all tests pass in CI

### 4. Post-Merge Actions

#### After PR is Merged
```bash
# Switch back to develop and update
git checkout develop
git pull origin develop

# Delete local feature branch
git branch -d feature/branch-name

# Verify remote branch was auto-deleted (should be)
git remote prune origin
```

### 5. Quality Gates

#### Automated Checks (GitHub Actions)
- **Conventional commits**: Enforced via commitlint
- **Build verification**: All projects must compile
- **Test execution**: All tests must pass
- **Branch naming**: Must follow git-flow patterns

#### Manual Review Points
- Code follows established conventions
- Tests provide adequate coverage
- Documentation updated if needed
- No breaking changes without proper communication
- Security considerations addressed

### 6. Release Process (Admin Only)

#### When Ready for Production
```bash
# Create release PR from develop to main
gh pr create --base main --head develop --title "release: version x.x.x"

# Use "Create a merge commit" for develop → main
# This preserves the git-flow history
```

## Emergency Hotfix Process

```bash
# Create hotfix branch from main
git checkout main
git pull origin main
git checkout -b hotfix/critical-fix-description

# Make minimal fix, commit, and create PR to main
git commit -m "hotfix: critical issue description"
gh pr create --base main --title "hotfix: critical issue"

# After merge to main, also merge to develop
gh pr create --base develop --head main --title "chore: merge hotfix to develop"
```

This ensures all completed work follows the established git-flow methodology and maintains code quality standards.