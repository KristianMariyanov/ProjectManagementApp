# Project Management App

Bare project skeleton for a project management app.

Teams create projects, add members, and track work as tasks with status,
priority, assignee, and due date. Authentication uses JWT access and refresh
tokens.

## Prerequisites

| Tool | Version |
|------|---------|
| [.NET SDK](https://dotnet.microsoft.com/download) | 10 |
| [Node.js](https://nodejs.org) | 20+ |

## Setup

Restore .NET tools and packages, create the SQLite database, then install frontend dependencies:

```bash
dotnet tool restore
cd api && dotnet restore && dotnet tool run dotnet-ef database update --project ProjectManagement.Api
cd ../frontend && npm install
```

## Run

Open two terminals:

```bash
cd api
dotnet run --project ProjectManagement.Api
```

```bash
cd frontend
npm run dev
```

Backend: `http://localhost:5000`

Frontend: `http://localhost:5173`

Seed credentials:

- `alice@example.com` / `Password1!`
- `bob@example.com` / `Password1!`

## Tests

Backend:

```bash
cd api
dotnet test
```

Frontend:

```bash
cd frontend
npm run build
npm run lint
npx playwright test
```

## Adding Workshop Assets

The facilitator materials, AI instructions, tool configs, playbooks, backlog,
and review examples live outside this bare skeleton in:

`..\workshop-assets`

See `..\workshop-assets\README.md` for the copy-back commands.
