# CommentsSPATask

`CommentsSPATask` is a full-stack threaded comments application built with ASP.NET Core, Entity Framework Core, SQL Server, React, and Docker Compose.

The project is structured as a typical layered .NET solution with separate API, application, domain, infrastructure, frontend, and test projects.

## Purpose

The application implements a public comments board where users can:

- browse root comments in a sortable, paged table
- open full reply threads
- create root comments
- reply to existing comments
- attach image or text files
- preview attachments in the UI
- solve a CAPTCHA before posting
- receive realtime comment updates through SignalR

## Implemented functionality

### Comments

- root comments are displayed on the main page
- replies are displayed as a nested thread
- replies are created through a dedicated endpoint
- default root comment ordering is newest first
- main page supports server-side paging with 25 root comments per page
- main page supports sorting by:
  - `User Name`
  - `E-mail`
  - `Created At`

### Form validation

The comment form validates both on the client and server:

- `User Name` is required and restricted to latin letters and digits
- `E-mail` is required and must be a valid email
- `Home page` is optional and must be a valid URL when provided
- `CAPTCHA` is required
- `Text` is required

### Allowed HTML in comments

Comment text only allows these tags:

- `<a href="" title=""></a>`
- `<code></code>`
- `<i></i>`
- `<strong></strong>`

The backend validates and sanitizes the content to block unsupported tags, invalid markup, and unsafe links.

### Attachments

Supported attachment types:

- images: `JPG`, `JPEG`, `PNG`, `GIF`
- text files: `TXT`

Rules:

- text files must be `<= 100 KB`
- images larger than `320x240` are resized proportionally before saving
- only validated and processed files are persisted

### Realtime updates

The application uses SignalR to notify connected clients when:

- a comment is created
- attachment-related comment updates must invalidate the current comments view

### Security-related behavior

The application includes protection-oriented behavior for:

- XSS mitigation through restricted HTML policy and sanitization
- SQL injection resistance through EF Core and parameterized queries
- CAPTCHA validation before accepting a comment
- server-side validation even if client-side validation is bypassed

### Background processing

The application currently runs a background cleanup job for expired captchas:

- old expired captcha rows are removed periodically
- cleanup also runs after startup

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
├── compose.override.yaml
├── README.md
└── SMOKE_TESTS.md
```

## Technology stack

### Backend

- .NET 10
- ASP.NET Core minimal APIs
- Entity Framework Core
- SQL Server
- MediatR
- Serilog
- SignalR

### Frontend

- React 19
- TypeScript
- Vite
- Redux Toolkit Query
- Bootstrap

### Tooling

- Git
- Docker
- Docker Compose
- xUnit
- Moq

## API overview

Main endpoints:

- `GET /api/comments`
  - returns a paged root comment list
- `GET /api/comments/{id}`
  - returns a full thread
- `POST /api/comments`
  - creates a root comment
- `POST /api/comments/{parentId}/replies`
  - creates a reply to an existing comment
- `POST /api/captchas`
  - creates a captcha challenge
- `GET /uploads/{storedFileName}`
  - serves uploaded attachments
- `GET /swagger`
  - Swagger UI
- `/hubs/comments`
  - SignalR hub

## Running the project

### Full stack with Docker

From the solution root:

```powershell
docker compose up --build
```

Default ports:

- client: `http://localhost:8080`
- API: `http://localhost:8081`
- SQL Server: `localhost:1433`

### API only

```powershell
dotnet run --project CommentsSPATask.Api
```

Development behavior:

- root API URL redirects to Swagger
- development settings come from `appsettings.Development.json`

### Frontend only

From `CommentsSPATask.Client`:

```powershell
npm install
npm run dev
```

## Docker notes

- SQL Server uses a health check before the API starts
- API startup retries migrations on transient SQL startup errors
- the frontend is built into a static Nginx container
- Vite environment variables are passed through Docker build arguments

## Testing

Unit tests:

```powershell
dotnet test CommentsSPATask.UnitTests\CommentsSPATask.UnitTests.csproj
```

Frontend production build:

```powershell
cd CommentsSPATask.Client
npm run build
```

## Smoke test checklist

The project smoke checklist is documented in:

- [SMOKE_TESTS.md](SMOKE_TESTS.md)

It contains a checkbox table with test IDs, preconditions, execution steps, and expected results.

## Schema

Database schema artifacts are stored in:

- [SchemeFiles](SchemeFiles)

Available files:

- [CommentsSPATask-schema.sql](SchemeFiles/CommentsSPATask-schema.sql)
  - SQL Server schema script generated from Entity Framework migrations
- [MySQLSchem.mwb](SchemeFiles/MySQLSchem.mwb)
  - MySQL Workbench model file for visual review
- [Scheme Image.png](SchemeFiles/SchemeImage.png)
  - exported schema diagram image

Important note:

- the runtime database used by the application is SQL Server
- the `.mwb` file is a review artifact for the task requirement
- the `.sql` file is the authoritative schema script generated from the implemented EF Core model

## Important implementation notes

- captcha images are stored in memory for the current application instance
- attachments are stored on disk under the uploads directory
- text files are served as UTF-8
- client API and hub endpoints are configured through Vite environment variables

## Current repository deliverables

- source code for backend and frontend
- Docker Compose setup
- root `README.md`
- smoke test document
- unit tests
- database schema artifacts
