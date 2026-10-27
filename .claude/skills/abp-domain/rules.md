# Domain layer rules

- Never reference EF Core or Application types from Domain.
- Reference other aggregates by Id only (e.g. `ManagerId`), never by navigation property.
- Normalise input in the entity (`Trim`, `ToUpperInvariant` for codes) before validating.
- A rule that needs a repository goes into the Manager, never into the entity.
- Throw `BusinessException(code).WithData(...)`; no plain exceptions, no hard-coded messages.
- Do not change app service, DTO, or EF Core files.
