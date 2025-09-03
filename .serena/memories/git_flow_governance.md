# Git Flow and Repository Governance

## Branch Strategy

### Main Branches
- **`main`**: Production-ready code, releases only
- **`develop`**: Integration branch for features, default branch

### Feature Branches
- **Naming**: `feature/`, `bugfix/`, `hotfix/`, `release/`
- **Source**: Created from `develop`
- **Target**: Merge back to `develop`
- **Lifespan**: Short-lived, deleted after merge

### Branch Protection Rules
- **Direct commits blocked**: No direct commits to `main` or `develop`
- **PR required**: All changes must go through pull requests
- **CI checks required**: All automated checks must pass

## Commit Standards

### Conventional Commits (Enforced)
Format: `type(scope): description`

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (no functional changes)
- `refactor`: Code refactoring
- `test`: Adding or modifying tests
- `chore`: Maintenance tasks
- `ci`: CI/CD changes

**Examples:**
```
feat: add user authentication
fix(auth): resolve login timeout issue
docs: update API documentation
test(user): add unit tests for user service
refactor: simplify data access layer
chore: update dependencies
```

## Merge Strategies

### Feature → Develop
- **Method**: Squash and merge
- **Reason**: Clean linear history in develop
- **Branch cleanup**: Auto-delete after merge

### Develop → Main
- **Method**: Create a merge commit
- **Reason**: Preserve git-flow structure
- **Timing**: Only for releases

## Automated Governance

### GitHub Actions (.github/workflows/pr-validation.yml)
- **Conventional commits**: Validated via commitlint
- **Branch naming**: Must match git-flow patterns
- **Git-flow compliance**: Blocks direct PRs to main
- **Build verification**: Ensures projects compile

### Pull Request Template
- **Merge strategy reminders**: Guides correct merge method
- **Conventional commit requirements**: Title format enforcement
- **Testing checklist**: Ensures tests are run and pass
- **Git-flow compliance**: Validates branch targeting

## Workflow Examples

### Standard Feature Development
1. `git checkout develop && git pull`
2. `git checkout -b feature/my-feature`
3. Make changes, test locally
4. `git commit -m "feat: add my feature"`
5. `git push -u origin feature/my-feature`
6. Create PR to `develop` via GitHub
7. Use "Squash and merge" after approval
8. Branch auto-deletes

### Release Process
1. All features merged to `develop`
2. Create PR from `develop` to `main`
3. Title: `release: version x.x.x`
4. Use "Create a merge commit" after approval
5. Tag release in `main`

### Emergency Hotfix
1. `git checkout main && git pull`
2. `git checkout -b hotfix/critical-issue`
3. Make minimal fix
4. `git commit -m "hotfix: resolve critical issue"`
5. Create PR to `main`
6. After merge, sync back to `develop`

## Repository Settings (Admin Configuration)
- **Default branch**: `develop`
- **Merge buttons**: Squash + Merge commit enabled, Rebase disabled
- **Auto-delete branches**: Enabled
- **Require PR reviews**: Recommended for team projects
- **Dismiss stale reviews**: Enabled
- **Require status checks**: All CI checks must pass