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
├── compose.deploy.yaml
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

## Deployed app

- frontend: [http://20.52.17.70:8080/](http://20.52.17.70:8080/)
- API: [http://20.52.17.70:8081/](http://20.52.17.70:8081/)
- Swagger: [http://20.52.17.70:8081/swagger](http://20.52.17.70:8081/swagger)

## Running the project

### Full stack with Docker

From the solution root:

1. Create a local `.env` file in the repository root.
2. Add the SQL Server password variable.
3. Start the stack.

Example `.env`:

```env
SA_PASSWORD=TestPassword123!
```

You can copy the example file:

```powershell
Copy-Item .env.example .env
```

```cmd
copy .env.example .env
```

```bash
cp .env.example .env
```

Then run:

```powershell
docker compose up --build
```

Default ports:

- client: `http://localhost:8080`
- API: `http://localhost:8081`
- SQL Server: `localhost:1433`

Docker persistence:

- SQL Server data is stored in the `sqlserver-data` volume
- uploaded attachments are stored in the `uploads-data` volume
- rebuilding the API container does not remove persisted database rows or uploaded files stored in those volumes

Notes:

- `.env` is required for Docker Compose because the SQL Server password is injected into both the SQL container and the API connection string
- the SQL password must satisfy SQL Server policy requirements:
  - at least 8 characters
  - uppercase letter
  - lowercase letter
  - number
  - symbol
- if you change `SA_PASSWORD` after the `sqlserver-data` volume was already created, remove existing volumes before restarting:

```powershell
docker compose down -v
docker compose up --build
```

### Full stack on a VM/VDS

The project can be deployed to a Linux VM/VDS with Docker Compose. This is the intended hosted deployment mode for keeping SQL Server as the runtime database.

The VM deployment uses the same Docker images as local development, with `compose.deploy.yaml` applying deployment-specific settings:

- the frontend is built with the VM public API URL
- the API receives the VM frontend origin through `Cors__AllowedOrigins__0`
- SQL Server is not published to the public internet
- uploaded files and SQL Server data are persisted through Docker volumes

Deployment is automated by the GitHub Actions workflow:

- `.github/workflows/deploy-vm.yml`

On push to `main`, the workflow connects to the VM over SSH, clones or updates the repository, writes the deployment `.env` file, and runs:

```bash
sudo docker compose -f compose.yaml -f compose.deploy.yaml up --build -d
```

The workflow writes these deployment values into `.env` on the VM:

- `SA_PASSWORD` from the GitHub `SA_PASSWORD` secret
- `VM_PUBLIC_IP` from the GitHub `VM_HOST` secret
- `SWAGGER_ENABLED` from the GitHub `SWAGGER_ENABLED` variable

When `SWAGGER_ENABLED=true`, Swagger is exposed in production for review at `/swagger`.

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
- local Docker Compose uses `compose.override.yaml` automatically
- VM deployment should use `compose.yaml` plus `compose.deploy.yaml`
- additional frontend origins can be passed to the API through `Cors__AllowedOrigins__0`, `Cors__AllowedOrigins__1`, and so on

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
