# Mail Marketing System

Mail Marketing System is a full-stack email marketing application built with .NET 8 Web API, Angular, PostgreSQL, and Entity Framework Core. It was developed for a mail marketing project brief: public subscription, custom user authentication, subscriber management, HTML email templates, SMTP configuration, asynchronous campaign sending, and delivery reports.

## Features

- Public landing page for email subscription
- Custom user registration, login, profile update, and password reset
- Password validation with encrypted storage
- Dashboard with subscriber, template, and sent-mail counts
- Subscriber list with email/date filtering and protected deletion
- HTML template management with WYSIWYG editor
- Template activation/deactivation
- Campaign sending to selected subscribers with async queue
- SMTP configuration screen
- Delivery log reporting by template, date range, and status
- User management screen with active/passive status

## Subscription Model

Public subscriptions are stored in a shared application-wide subscriber pool. This means an email entered on the landing page is visible in the management panel for every active admin user.

Campaign ownership still belongs to the logged-in admin:

- SMTP settings are read from the logged-in admin account.
- Templates are managed by the logged-in admin account.
- Subscribers are selected from the shared subscriber pool.
- Delivery logs are reported under the selected template and sending admin.

## Tech Stack

- Backend: .NET 8 ASP.NET Core Web API
- Frontend: Angular
- Database: PostgreSQL
- ORM: Entity Framework Core
- Auth: Custom JWT authentication
- Email: SMTP via `System.Net.Mail`
- Tests: xUnit

## Project Structure

```text
MailMarketing.API          API controllers, program setup, background worker
MailMarketing.Business     Services and DTOs
MailMarketing.Core         Shared utilities, queue, encryption
MailMarketing.DataAccess   DbContext, EF Core configurations, migrations
MailMarketing.Entities     Domain entities
MailMarketing.Tests        Backend tests
MailMarketing.UI           Angular frontend
```

## Requirements

- .NET 8 SDK
- Node.js and npm
- PostgreSQL

## Database

Default connection string is in `MailMarketing.API/appsettings.json`:

```json
"DefaultConnection": "Host=localhost;Port=5432;Database=mail_pazarlama_db;Username=mailapp_user;Password=MailApp_2026_local!"
```

Create matching PostgreSQL database/user or update the connection string for your local environment.

Run migrations:

```bash
dotnet ef database update --project MailMarketing.DataAccess --startup-project MailMarketing.API
```

## Run Backend

```bash
dotnet run --project MailMarketing.API/MailMarketing.API.csproj --urls http://localhost:5281
```

Swagger:

```text
http://localhost:5281/swagger
```

## Run Frontend

```bash
cd MailMarketing.UI
npm install
npm start -- --host 127.0.0.1 --port 4200
```

Frontend:

```text
http://127.0.0.1:4200
```

The Angular app expects the API at:

```text
http://localhost:5281/api
```

This value is configured in `MailMarketing.UI/src/environments/environment.ts`.

## Test

Backend tests:

```bash
dotnet test MailMarketingSystem.sln --no-restore
```

Frontend production build:

```bash
cd MailMarketing.UI
npm run build
```

## Notes

- Mail sending uses SMTP settings saved from the management panel.
- Campaign sending is queued and processed by a background worker.
- Mail send timeout is configured with `MailSending:TimeoutSeconds`.
- GitHub delivery branch is `main`.
