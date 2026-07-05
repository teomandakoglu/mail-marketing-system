# Backend Skeleton Design

## Scope

Create the initial .NET 8 backend solution for the Mail Marketing System. This step only establishes the solution, layer projects, references, EF Core 8 package baseline, and folder structure needed before entities, DbContext, migrations, and business services are implemented.

## Architecture

The backend uses a layered architecture:

- `MailMarketing.Entities`: plain entity classes.
- `MailMarketing.Core`: shared utilities, result models, security abstractions, and cross-layer contracts.
- `MailMarketing.DataAccess`: Entity Framework Core, PostgreSQL provider, DbContext, entity configurations, and repositories.
- `MailMarketing.Business`: DTOs, validation rules, and business services.
- `MailMarketing.API`: ASP.NET Core Web API, controllers, dependency injection, configuration, and startup surface.

Dependencies flow inward-to-outward without cycles. `Core` and `Entities` take no project references. `DataAccess` references `Core` and `Entities`. `Business` references `DataAccess`, `Entities`, and `Core`. `API` references `Business` and `Core`.

## Explicit Future Constraints

- Passwords must follow the project form requirement for reversible encryption. The implementation must use a custom AES-based crypto service, not one-way password hashing, because the form explicitly requires decryptability.
- Asynchronous mail sending must use a durable background processing design such as `IHostedService` plus persisted send records, or Hangfire. Controllers must not use ad hoc `Task.Run()` for mail dispatch.
- EF Core packages must remain on version `8.0.*` to match the installed .NET 8 SDK and `dotnet-ef` tool.
