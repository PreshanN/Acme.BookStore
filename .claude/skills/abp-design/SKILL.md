---
name: abp-design
description: Plan a new feature for the Acme.BookStore ABP solution without editing application code. Produces a design covering entities, business rules, permissions, DTOs, application services, error codes, affected files, migration impact on existing data, and open questions, then stops for approval.
disable-model-invocation: true
argument-hint: "<feature to design, e.g. employee probation period>"
---

# ABP Design

Design the feature described in `$ARGUMENTS` for this solution. This skill is read-only with respect to application code: inspect, plan, and report. Do not create or edit files under `src/` or `test/`, do not add migrations, and do not run DbMigrator or the Web app. Writing the design to the scratchpad directory is fine if it is long; the chat reply must still carry the full design.

## Ground rules

- Read `CLAUDE.md` first. It describes the module chain, the Employee Management domain, the test topology, and the known gaps in the current tree. Treat its "Current state" section as a list of things to verify, not as facts.
- `.cursor/rules/framework/common/ddd-patterns.mdc`, `application-layer.mdc`, `authorization.mdc`, `multi-tenancy.mdc`, and `.cursor/rules/framework/data/ef-core.mdc` are ABP's own guidance and match this solution. Follow them rather than inventing patterns.
- Use read-only git (`git status`, `git diff`, `git log`) to understand uncommitted work before designing on top of it. Say which branch and which uncommitted files the design assumes.
- Distinguish verified findings (you read the code) from assumptions (you inferred). Label them.

## What to inspect

Read the real files for the aggregate(s) the feature touches. For an Employee or Department feature that means at least:

- Aggregates and child entities in `src/Acme.BookStore.Domain/Employees/` and `src/Acme.BookStore.Domain/Departments/`, plus the value objects under `src/Acme.BookStore.Domain/Shared/`.
- Domain services `EmployeeManager` and `DepartmentManager`, which own cross-aggregate rules.
- Repository interfaces in Domain and their `EfCore*Repository` implementations.
- `BookStoreDbContext.OnModelCreating` and `EntityFrameworkCore/EntityTypeConfigurations/`. Only mappings that are actually applied count; check for `ApplyConfiguration` calls.
- The latest migration and `BookStoreDbContextModelSnapshot.cs` in `src/Acme.BookStore.EntityFrameworkCore/Migrations/`.
- Permissions in `src/Acme.BookStore.Application.Contracts/Permissions/BookStorePermissions.cs` and `BookStorePermissionDefinitionProvider.cs`.
- Existing DTOs, `I*AppService` interfaces, the `*AppService` implementations, the shared `BookStoreAppService` base class, and the Mapperly mappers (`MapperBase<TSource, TDest>` subclasses).
- Existing tests under `test/`, so the design can name the test class that should cover the feature.

## Design document structure

Produce these sections in this order. Keep each tight; a reader will implement from it.

1. **Summary.** Two or three sentences: what the feature does and which aggregate owns it.
2. **Current state.** What exists today that the feature builds on, with file paths. Note any defect or inconsistency in that code that the feature would inherit.
3. **Entities and value objects.** New or changed properties, backing fields, invariants, and which aggregate method sets each one. State changes go through methods on the aggregate, never public setters. Say whether anything belongs in a value object or a child entity instead of the root.
4. **Business rules.** One line per rule: the rule, where it is enforced (aggregate method, domain manager, or app service), and the `BookStore:*` error code it throws via `BusinessException`. Cross-aggregate rules belong in the domain manager.
5. **Permissions.** New constants under `BookStorePermissions`, their parent group, and the localization keys for the definition provider. Say which app-service methods carry which permission.
6. **DTOs and validation.** Each new or changed DTO in Application.Contracts, its properties, and its data-annotation validation. Remember that the FluentValidation validators in Application are not wired into ABP; do not rely on them.
7. **Application service.** New or changed methods on the `I*AppService` interface and implementation, their signatures, authorization attributes, and which domain manager or aggregate method each one calls. Note that every public method becomes an auto-API endpoint. Note any salary-redaction or similar response-shaping rule the method must respect.
8. **Mapping.** Which Mapperly mapper changes, and any property that needs an explicit mapping.
9. **Persistence and migration impact.** EF mapping changes (columns, owned types, indexes, table names with the `App` prefix). Then the migration impact on existing data: nullable or defaulted columns, backfill needs, index uniqueness against existing rows, and anything that drops or renames. Say explicitly whether the current snapshot and latest migration match the model before this feature; if they do not, say so and stop the migration plan there, because a new migration generated on a mismatched model carries the mismatch forward.
10. **Multi-tenancy and auditing.** Confirm `IMultiTenant` behaviour, tenant filters on any new query, and whether entity history should cover the change.
11. **Affected files.** A list of every file to create or modify, grouped by project, each with a one-line reason. Include the test class.
12. **Tests.** The scenarios a test in `Acme.BookStore.Application.Tests` (run through `Acme.BookStore.EntityFrameworkCore.Tests` on SQLite) should cover. Flag any repository path that uses `EF.Functions.ILike`, which SQLite tests cannot exercise.
13. **Open questions.** Anything the user must decide. Give a recommended answer for each.

## Stop for approval

End the reply with a short list of decisions you need and a single line asking for approval to implement. Do not begin implementation in the same turn, even if the design seems obvious. If the user approves with changes, restate the changed design points before touching any file.
