# CommentsSPATask

`CommentsSPATask` is a full-stack threaded comments application built with ASP.NET Core minimal APIs and a React client.

The project demonstrates a typical business application structure with separate API, application, domain, infrastructure, frontend, and test projects.

## What the project does

The application lets users:

- view a paged list of root comments
- sort comments by different fields
- open a full thread for a selected comment
- create root comments
- create replies for existing comments
- attach image or text files to comments
- preview uploaded attachments in the UI
- solve a captcha before posting
- receive realtime updates when new comments are added

## Main features

### Backend

- minimal API endpoints grouped by feature
- MediatR-based application layer
- domain-driven separation between domain, application, and infrastructure
- EF Core with SQL Server
- Swagger/OpenAPI documentation
- SignalR hub for realtime comment notifications
- HTML sanitization for comment text
- file upload storage and static file serving for attachments
- captcha generation, image rendering, persistence, validation, and cleanup
- unit test coverage for endpoints and application handlers

### Frontend

- React + TypeScript + Vite
- Redux Toolkit Query for API access and caching
- threaded comments UI
- modal form for creating comments and replies
- attachment preview support
- SignalR client listener for realtime updates
- query-string based pagination and sorting
- Dockerized static hosting with Nginx

## Solution structure

```text
CommentsSPATask
├── CommentsSPATask.Api
├── CommentsSPATask.Application
├── CommentsSPATask.Client
├── CommentsSPATask.Domain
├── CommentsSPATask.Infrastructure
├── CommentsSPATask.UnitTests
├── compose.yaml
└── compose.override.yaml
```

## Tech stack

- .NET 10
- ASP.NET Core minimal APIs
- MediatR
- Entity Framework Core
- SQL Server
- Serilog
- SignalR
- React 19
- TypeScript
- Redux Toolkit Query
- Bootstrap
- Docker Compose

## Running locally

### Backend only

Run the API project:

```powershell
dotnet run --project CommentsSPATask.Api
```

By default in development:

- the API uses `appsettings.Development.json`
- Swagger is available from the root redirect

### Frontend only

From `CommentsSPATask.Client`:

```powershell
npm install
npm run dev
```

### Full stack with Docker

From the solution root:

```powershell
docker compose up --build
```

Default local ports:

- client: `http://localhost:8080`
- API: `http://localhost:8081`
- SQL Server: `localhost:1433`

## Important API capabilities

- `GET /api/comments` returns paged root comments
- `GET /api/comments/{id}` returns a full thread
- `POST /api/comments` creates a root comment
- `POST /api/comments/{parentId}/replies` creates a reply
- `POST /api/captchas` creates a captcha challenge
- `/uploads/{storedFileName}` serves uploaded attachments
- `/hubs/comments` provides realtime SignalR updates

## Notes

- Captcha images are stored in memory and are intended for the current application instance.
- Attachments are stored on disk and exposed through the uploads path.
- The client build depends on Vite environment variables for API and hub endpoints.
