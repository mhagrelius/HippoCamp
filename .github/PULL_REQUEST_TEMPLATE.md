# Pull Request Template

> **Merge Strategy Reminder:**
> - **Feature → `develop`**: Use "Squash and merge" 
> - **Hotfix → `main`**: Use "Squash and merge"
> - **`develop` → `main`**: Use "Create a merge commit" (releases only)
> 
> Use Conventional Commits in the title (e.g., `feat: add user authentication`, `fix(auth): resolve login issue`).

## Summary
- What is the goal of this PR? Why now?

## Changes
- Key changes in this PR (bulleted, concise)
- Note any breaking changes, migrations, or config updates

## How To Test
```bash
# Instructions for testing this change
```

## Checklist
- [ ] Title follows Conventional Commits format
- [ ] **Correct merge strategy** selected (see reminder above)
- [ ] Build passes locally
- [ ] Tests added/updated and pass
- [ ] No secrets, keys, or credentials committed
- [ ] Breaking changes documented
- [ ] Linked issues referenced (e.g., `Closes #123`)

## Git-Flow Compliance
- [ ] Feature branches target `develop` (not `main`)
- [ ] Hotfix branches target `main` (emergency fixes only)
- [ ] Release PRs are from `develop` to `main`
- [ ] Branch follows naming convention (feature/, hotfix/)
- [ ] Issue number referenced in branch name (e.g., feature/15-description)

## Additional Notes
- Architecture decisions, follow-ups, or out-of-scope items
