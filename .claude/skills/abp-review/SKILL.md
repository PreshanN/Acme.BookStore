---
name: abp-review
description: Review Git changes in the Acme.BookStore ABP solution against a base branch (default master; this repository has no main). Checks ABP architecture, domain logic, authorization, validation, security, multi-tenancy, EF Core, migrations, Mapperly, and build health, and reports severity, file and line, evidence, and suggested fixes. Read-only.
disable-model-invocation: true
argument-hint: "[base-branch, default master]"
---

# ABP Review

Review the changes on the current branch against the base branch given in `$ARGUMENTS`. If no argument is given, the base is `master`. This repository has no `main` branch; do not assume one.

This skill is read-only. Do not edit, stage, unstage, commit, stash, or revert anything. Do not add, apply, or roll back migrations, and do not run DbMigrator or the Web app. A `dotnet build` is allowed because it only writes to the git-ignored `bin` and `obj` folders.

## Scope the diff

Use read-only git only. Collect all three layers of change and say which layer each finding comes from:

```bash
git status --porcelain=v1 -b
git diff <base>...HEAD --stat                 # committed on this branch
git diff --cached                             # staged
git diff                                      # unstaged
git ls-files --others --exclude-standard      # untracked
```

Read every changed file in full, not just the hunks. Read the HEAD version too when a change removes something, because removals are where regressions hide. For untracked files there is no diff, so read the whole file.

## Checklist

Work through every item. Each one names the thing that has gone wrong in this codebase before or is an ABP rule this solution relies on.

**ABP architecture**
- Layer direction holds: Domain.Shared -> Domain -> Application -> Web; Application.Contracts and HttpApi only depend on what `CLAUDE.md` shows. No Application type referenced from Domain, no EF Core type outside the EF Core project.
- No hand-written controllers; app services are exposed by auto-API. Every public method on an app service, including ones on the shared `BookStoreAppService` base class, becomes an endpoint on every derived service. Flag public base-class methods that leak unrelated endpoints or route conflicts.
- `BookStoreAppService` keeps `LocalizationResource = typeof(BookStoreResource)`. Flag its removal.
- Derived app services do not redeclare fields the base already provides. Flag CS0108/CS0114 hiding and CS0649 never-assigned fields; these have caused null references at runtime here.
- Mapperly is the mapper. New mappings are `MapperBase<TSource, TDest>` subclasses, not AutoMapper profiles.

**Domain logic**
- Aggregates are created through `EmployeeManager` or `DepartmentManager`, never `new` in an app service. Cross-aggregate rules live in the manager.
- State changes go through aggregate methods that validate and throw `BusinessException` with `BookStore:*` codes. Flag public setters on aggregate state, `Hr:*` or other foreign code prefixes, and rules enforced only in the app service.
- Child collections stay private lists with read-only exposure and `HasField` mapping where the field name is not conventional.
- Value objects are immutable and compared by value.

**Authorization**
- Class-level `[Authorize(BookStorePermissions.X.Default)]` plus a per-method permission on every mutating method. Flag any mutating method with only the class-level attribute.
- New permissions are defined in both `BookStorePermissions` and `BookStorePermissionDefinitionProvider` with localization keys.
- Salary fields are redacted unless the caller holds `BookStore.Employees.ViewSalary`. Any method that returns `EmployeeDto`, including methods on the base class, must go through the redacting path.

**Validation**
- DTOs use data annotations. Flag new DTOs with no validation on required strings, amounts, or GUIDs.
- The FluentValidation validators in Application are not executed by ABP (no `Volo.Abp.FluentValidation` module). Flag any change that relies on them.

**Security**
- No secrets or connection strings in code. Input used in dynamic sorting (`System.Linq.Dynamic.Core`) must come from a known-safe source or be validated. Filters passed to `EF.Functions.ILike` are parameterised; flag string concatenation into raw SQL.

**Multi-tenancy**
- New entities implement `IMultiTenant` with a private-set `TenantId`. Seed contributors wrap work in `_currentTenant.Change(context?.TenantId)`. Unique indexes include `TenantId`.

**EF Core**
- Every entity registered in `OnModelCreating` is a real domain entity. Flag any `builder.Entity<T>` where `T` is a configuration or helper class.
- `IEntityTypeConfiguration` classes under `EntityTypeConfigurations/` count only if an `ApplyConfiguration` call exists; flag a configuration that is both inline and applied, and flag applied configurations that diverge from the inline block they replace.
- Tables use the `App` prefix via `BookStoreConsts.DbTablePrefix`. Owned types keep their explicit column names (`SalaryAmount`, `Email`, `AddressLine1`, `ContactEmail`).
- `ConfigureByConvention()` is called on every entity. Npgsql legacy timestamp setting stays in sync between the EF module and the design-time factory.

**Migrations**
- The snapshot matches the model. If the model changed and no migration was added, or a migration exists but the model moved on, say so.
- Read every `Up` method for `DropColumn`, `DropTable`, `DropForeignKey`, and `RenameColumn`. Report each as a data-loss risk with the table and column named. Check `Down` restores the data, not just the schema.
- A migration generated from a broken model (wrong entity registered, stray unprefixed table) is a blocker, not a warning.
- Remember the Web app also migrates on startup, so a bad migration reaches the dev database the moment the app runs.

**Build health**
- Run `dotnet build` on the changed projects with `--no-incremental` so warnings are not hidden by an up-to-date check. If the full build fails with MSB3021/MSB3027 on the Web project, IIS Express holds the DLLs; report that as an environment issue and build the non-Web projects instead.
- Report every CS0108, CS0114, CS0649, and CS8618 warning in changed files. Other pre-existing warnings go in a single summary line.
- Do not run the EF Core tests unless asked; if asked, they use in-memory SQLite and touch no real database.

## Report format

Lead with a one-line verdict: ready, ready with fixes, or blocked. Then list findings ordered by severity. For each finding:

- **Severity**: Blocker (data loss, runtime failure, security hole), Major (wrong behaviour or broken convention that will spread), Minor (warning, style, naming), Note (observation, no action required).
- **Location**: file path and line, as a clickable `path:line` reference. For untracked files say "untracked".
- **Evidence**: the exact code or compiler output that shows the problem. Quote it; do not paraphrase.
- **Why it matters**: one or two sentences.
- **Suggested fix**: the smallest change that resolves it, described in words or a short snippet. Do not apply it.

After the findings, list what was verified and found clean, so the reader knows the checklist was covered rather than skipped. Finish with anything you could not verify and why, such as a locked build or a database you did not query.
