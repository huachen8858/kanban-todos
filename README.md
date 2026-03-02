# TaskFlow

A full-stack task management system with a Kanban board UI.

**Stack**: .NET 9 Web API · Nuxt 4 · MySQL 8 · Nginx · Docker

---

## Quick Start

### Prerequisites

- [Docker](https://docs.docker.com/get-docker/) and Docker Compose

### Run with Docker

```bash
# 1. Clone the repo
git clone <repo-url>
cd kanban-todos

# 2. Configure environment (edit JWT_SECRET before production use)
cp .env .env.local  # optional backup

# 3. Build and start all services
docker-compose up --build
```

Once running, open **http://localhost** in your browser.

> The first build takes a few minutes while Docker pulls images and compiles the app.

### Service URLs

| Service | URL |
|---|---|
| App (Nuxt) | http://localhost |
| API (health) | http://localhost/api/health |
| Swagger UI | http://localhost/swagger |

### Test Account

A test account is pre-registered in the database (if you ran the dev server previously):

| Field | Value |
|---|---|
| Email | test@taskflow.com |
| Password | Test1234 |

Or register a new account at http://localhost/register.

---

## Development Mode

Run each service individually for hot-reload development.

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- MySQL running (Docker or local)

### 1. Start MySQL only

```bash
docker-compose up mysql -d
```

### 2. Run the backend

```bash
cd backend
dotnet run --project TaskFlow.API
# API available at http://localhost:5000
# Swagger at http://localhost:5000/swagger
```

### 3. Run the frontend

```bash
cd frontend
npm install
npm run dev
# App available at http://localhost:3000
```

The Nuxt dev server proxies `/api/*` → `http://localhost:5000/api` automatically (no CORS issues).

---

## Architecture

```
Browser
  └─ http://localhost (port 80)
       └─ Nginx
            ├─ /api/*    → backend:5000  (.NET 9 Web API)
            ├─ /swagger  → backend:5000
            └─ /*        → frontend:3000 (Nuxt 4 SSR)
                              └─ MySQL:3306
```

### Key Design Decisions

- **JWT auth** stored in SSR-safe cookies (`useCookie`) — works on server and client
- **Kanban drag-and-drop** via `vue-draggable-plus` (SortableJS wrapper for Vue 3)
- **EF Core** with Pomelo MySQL provider; enums serialized as strings
- **Repository pattern** + service layer in the backend
- **FluentValidation** for all request DTOs; global exception middleware for consistent error envelopes

---

## API Overview

Base URL: `/api/v1`

| Method | Path | Description |
|---|---|---|
| POST | `/auth/register` | Register a new user |
| POST | `/auth/login` | Login, returns JWT |
| GET | `/projects` | List user's projects |
| POST | `/projects` | Create a project |
| GET | `/projects/{id}` | Get project details |
| PUT | `/projects/{id}` | Update a project |
| DELETE | `/projects/{id}` | Delete a project |
| GET | `/projects/{id}/tasks` | List tasks in a project |
| POST | `/projects/{id}/tasks` | Create a task |
| GET | `/tasks/{id}` | Get task details |
| PUT | `/tasks/{id}` | Update a task |
| DELETE | `/tasks/{id}` | Delete a task |
| PATCH | `/tasks/{id}/status` | Update task status |
| GET | `/tasks/{id}/comments` | List comments on a task |
| POST | `/tasks/{id}/comments` | Add a comment |
| DELETE | `/comments/{id}` | Delete a comment |

All endpoints (except auth) require `Authorization: Bearer <token>` header.

---

## Environment Variables

Copy `.env` and adjust for your environment:

```env
MYSQL_ROOT_PASSWORD=rootpassword   # MySQL root password
MYSQL_DATABASE=taskflow            # Database name
MYSQL_USER=taskflow_user           # App DB user
MYSQL_PASSWORD=taskflow_pass       # App DB password

# IMPORTANT: Change to a strong random 256-bit value in production
JWT_SECRET=your-256-bit-secret-placeholder-change-me

NUXT_PUBLIC_API_BASE=http://localhost/api/v1
```
