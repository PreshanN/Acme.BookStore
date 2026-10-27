---
name: abp-domain
description: Generate the ABP Domain layer (aggregate changes, manager, error codes) for one entity from an approved design. Use after the design is approved.
argument-hint: "<Entity> @<consts file> @<design or rules>"
disable-model-invocation: true
allowed-tools: Read Grep Glob Bash(dotnet build *)
---

# ABP Domain

Implement the Domain.Shared and Domain layers for: $ARGUMENTS

Work only from an approved design (the output of `/abp-design`, or the design or rules file passed in `$ARGUMENTS`). Read `CLAUDE.md` and `rules.md` next to this skill before touching any file, and follow every rule in `rules.md`.

## What to produce

1. **`{Entity}Consts.cs` in Domain.Shared.** Every new max length or limit becomes a constant. Existing consts live in two places in this solution: `EmployeeConsts` is in `src/Acme.BookStore.Domain.Shared/Employees/`, while `DepartmentConsts` is in `src/Acme.BookStore.Domain/Shared/Departments/` (namespace `Acme.BookStore.Shared.Departments`). Put new constants in the file that already exists for the entity; only create a new file in `src/Acme.BookStore.Domain.Shared/<Entity>s/` when the entity has none. Do not move an existing consts file.
2. **Error codes** as constants in `src/Acme.BookStore.Domain.Shared/EmployeeManagementSystemDomainErrorCodes.cs` (namespace `Acme.BookStore`), using the `BookStore:` prefix, with a matching message for each code in `src/Acme.BookStore.Domain.Shared/Localization/BookStore/en.json`. Message placeholders must match the `WithData` keys.
3. **`{Entity}.cs`** in `src/Acme.BookStore.Domain/<Entity>s/`: `FullAuditedAggregateRoot<Guid>`, `IMultiTenant`, `[Audited]`, private setters, a protected parameterless constructor for EF Core, `Check.*` guards on every input, and one named method per state change. The Id is passed into the constructor; it is never generated inside the entity.
4. **`{Entity}Manager.cs`** in the same folder: a `DomainService` holding every rule that needs other data (uniqueness, references to other aggregates). It creates the aggregate with `GuidGenerator.Create()` and `CurrentTenant.Id`, and throws `BusinessException` with a code from `EmployeeManagementSystemDomainErrorCodes`. If a rule needs a repository method that does not exist, add it to the `I{Entity}Repository` interface in Domain and say so in the report; the EF Core implementation is out of scope for this skill.

## Steps

1. Read the approved design and the existing aggregate, manager, consts, repository interface, error codes and `en.json`.
2. Make the changes above, and nothing else. Do not change app service, DTO, or EF Core files, and do not add migrations.
3. Run `dotnet build src/Acme.BookStore.Domain` (which also builds Domain.Shared). If it fails with MSB3021/MSB3027 on the Web project, that is IIS Express holding files, not a compile error; report it and build the Domain project alone.
4. List every changed file and every ABP type used (`FullAuditedAggregateRoot<Guid>`, `IMultiTenant`, `DomainService`, `BusinessException`, `Check`, `IGuidGenerator`, `ICurrentTenant`, ...).
5. Stop. Do not start the EF Core mapping, migration, application service or UI work.
