# Backend Skeleton Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Create the initial .NET 8 layered backend solution for the Mail Marketing System.

**Architecture:** The solution contains five projects: `MailMarketing.Core`, `MailMarketing.Entities`, `MailMarketing.DataAccess`, `MailMarketing.Business`, and `MailMarketing.API`. References flow from API to Business to DataAccess/Core/Entities without circular dependencies.

**Tech Stack:** .NET 8, ASP.NET Core Web API, Entity Framework Core 8, Npgsql PostgreSQL provider.

---

### Task 1: Scaffold Solution and Projects

**Files:**
- Create: `MailMarketingSystem.sln`
- Create: `MailMarketing.Core/MailMarketing.Core.csproj`
- Create: `MailMarketing.Entities/MailMarketing.Entities.csproj`
- Create: `MailMarketing.DataAccess/MailMarketing.DataAccess.csproj`
- Create: `MailMarketing.Business/MailMarketing.Business.csproj`
- Create: `MailMarketing.API/MailMarketing.API.csproj`

- [ ] Create the solution with `dotnet new sln -n MailMarketingSystem`.
- [ ] Create four class libraries with `dotnet new classlib`.
- [ ] Create the API project with `dotnet new webapi`.
- [ ] Add all projects to the solution.

### Task 2: Add References and Packages

**Files:**
- Modify: `MailMarketing.DataAccess/MailMarketing.DataAccess.csproj`
- Modify: `MailMarketing.Business/MailMarketing.Business.csproj`
- Modify: `MailMarketing.API/MailMarketing.API.csproj`

- [ ] Add DataAccess references to `MailMarketing.Entities` and `MailMarketing.Core`.
- [ ] Add Business references to `MailMarketing.DataAccess`, `MailMarketing.Entities`, and `MailMarketing.Core`.
- [ ] Add API references to `MailMarketing.Business` and `MailMarketing.Core`.
- [ ] Install EF Core 8 and Npgsql EF Core provider packages.
- [ ] Install EF Core design/tooling packages in the API project.

### Task 3: Create Starter Folders and Verify

**Files:**
- Create directories under each layer for planned code organization.

- [ ] Create `Utilities/Security` and `Utilities/Results` under Core.
- [ ] Create `Concrete` under Entities.
- [ ] Create `Contexts`, `Configurations`, and `Repositories` under DataAccess.
- [ ] Create `Services`, `DTOs`, and `ValidationRules` under Business.
- [ ] Run `dotnet restore` and `dotnet build`.
