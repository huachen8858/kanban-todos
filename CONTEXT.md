# CONTEXT.md — TaskFlow Project Context

## Project Summary

**TaskFlow** is a personal task management system designed as a full-stack learning project.
It covers every layer of modern web development: REST API design, database schema, frontend SPA,
reverse proxy configuration, and Docker containerization.

---

## Business Domain

Users can manage **Projects** and **Tasks**. Each task belongs to a project and can have a priority, status, and due date.

### Core Entities

```
User
├── id, email, password_hash, name, created_at

Project
├── id, name, description, owner_id (FK → User), created_at, updated_at

Task
├── id, title, description
├── status: enum(Todo, InProgress, Done)
├── priority: enum(Low, Medium, High)
├── due_date (nullable)
├── project_id (FK → Project)
├── assignee_id (FK → User, nullable)
├── created_at, updated_at

TaskComment
├── id, content, task_id (FK → Task), author_id (FK → User), created_at
```

---

## Architecture Overview

```
Browser
   │
   ▼
┌─────────────────────────────────┐
│         Nginx (port 80)         │
│  /api/* → backend:5000          │
│  /*     → frontend:3000         │
└────────────┬────────────────────┘
             │
    ┌────────┴──────────┐
    ▼                   ▼
┌─────────┐        ┌──────────┐
│ Nuxt 3  │        │ .NET API │
│ (3000)  │        │  (5000)  │
└─────────┘        └────┬─────┘
                        │
                   ┌────▼─────┐
                   │ MySQL 8  │
                   │  (3306)  │
                   └──────────┘
```

### Layer Responsibilities

| Layer | Responsibility |
|---|---|
| Nginx | Reverse proxy, SSL termination (future), static files |
| Nuxt 3 Frontend | UI, client routing, API communication via composables |
| .NET Web API | REST endpoints, auth, request validation, business rules |
| Application Layer | Services, use cases, interfaces |
| Infrastructure | EF Core, MySQL repositories, migrations |
| MySQL 8 | Persistent storage |

---

## API Endpoints

### Auth

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/v1/auth/register` | Register new user |
| POST | `/api/v1/auth/login` | Login, returns JWT |

### Projects

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/v1/projects` | List all projects for current user |
| GET | `/api/v1/projects/{id}` | Get single project with tasks |
| POST | `/api/v1/projects` | Create project |
| PUT | `/api/v1/projects/{id}` | Update project |
| DELETE | `/api/v1/projects/{id}` | Delete project |

### Tasks

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/v1/projects/{projectId}/tasks` | List tasks in a project |
| GET | `/api/v1/tasks/{id}` | Get task detail |
| POST | `/api/v1/projects/{projectId}/tasks` | Create task |
| PUT | `/api/v1/tasks/{id}` | Update task |
| PATCH | `/api/v1/tasks/{id}/status` | Update task status only |
| DELETE | `/api/v1/tasks/{id}` | Delete task |

### Task Comments

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/v1/tasks/{taskId}/comments` | List comments |
| POST | `/api/v1/tasks/{taskId}/comments` | Add comment |
| DELETE | `/api/v1/comments/{id}` | Delete comment |

---

## Frontend Pages

```
/                   → Redirect to /dashboard or /login
/login              → Login page
/register           → Register page
/dashboard          → Overview: recent tasks, project summary cards
/projects           → Project list page
/projects/[id]      → Project detail: task board (Kanban columns)
/tasks/[id]         → Task detail: full edit form + comments
/profile            → User profile settings
```

### Component Structure

```
components/
├── ui/
│   ├── Button.vue
│   ├── Input.vue
│   ├── Modal.vue
│   └── Badge.vue
├── task/
│   ├── TaskCard.vue       # Kanban card
│   ├── TaskForm.vue       # Create/edit form
│   └── TaskCommentList.vue
└── project/
    ├── ProjectCard.vue
    └── KanbanBoard.vue
```

---

## Database Schema (MySQL 8)

```sql
CREATE TABLE users (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    email       VARCHAR(255) NOT NULL UNIQUE,
    name        VARCHAR(100) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    created_at  DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE projects (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    name        VARCHAR(200) NOT NULL,
    description TEXT,
    owner_id    INT NOT NULL,
    created_at  DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at  DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (owner_id) REFERENCES users(id)
);

CREATE TABLE tasks (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    title       VARCHAR(300) NOT NULL,
    description TEXT,
    status      ENUM('Todo', 'InProgress', 'Done') NOT NULL DEFAULT 'Todo',
    priority    ENUM('Low', 'Medium', 'High') NOT NULL DEFAULT 'Medium',
    due_date    DATE,
    project_id  INT NOT NULL,
    assignee_id INT,
    created_at  DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at  DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (project_id) REFERENCES projects(id) ON DELETE CASCADE,
    FOREIGN KEY (assignee_id) REFERENCES users(id)
);

CREATE TABLE task_comments (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    content     TEXT NOT NULL,
    task_id     INT NOT NULL,
    author_id   INT NOT NULL,
    created_at  DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (task_id) REFERENCES tasks(id) ON DELETE CASCADE,
    FOREIGN KEY (author_id) REFERENCES users(id)
);
```

---

## Docker Compose Services

```yaml
services:
  mysql:     # MySQL 8, port 3306, named volume for persistence
  backend:   # .NET 8 API, port 5000, depends on mysql
  frontend:  # Nuxt 3, port 3000, depends on backend
  nginx:     # port 80, proxies to backend and frontend
```

---

## Learning Plan (Phase by Phase)

### Phase 1 — Infrastructure Setup (Week 1)

**Goal**: Docker environment running, database accessible

- [ ] Set up `docker-compose.yml` with MySQL 8 service
- [ ] Connect MySQL Workbench to the Docker MySQL instance
- [ ] Run schema SQL to create tables
- [ ] Create `.NET 8 Web API` project with solution structure (API / Application / Domain / Infrastructure)
- [ ] Add EF Core + Pomelo MySQL provider
- [ ] Verify database connectivity from API

**Key Concepts**: Docker networking, volumes, EF Core DbContext, connection strings

---

### Phase 2 — Backend CRUD (Week 2)

**Goal**: All REST endpoints working, testable via Swagger / Postman

- [ ] Implement `User` entity and `AuthController` (register / login with JWT)
- [ ] Implement `Project` CRUD (controller → service → repository)
- [ ] Implement `Task` CRUD with filtering by status/priority
- [ ] Implement `TaskComment` endpoints
- [ ] Add FluentValidation for all request DTOs
- [ ] Add global exception handling middleware
- [ ] Add Swagger/OpenAPI documentation

**Key Concepts**: Repository Pattern, DTOs, JWT Auth, middleware, async/await

---

### Phase 3 — Frontend Foundation (Week 3)

**Goal**: Nuxt 3 app with working auth and project list

- [ ] Set up Nuxt 3 project with TypeScript + Tailwind CSS
- [ ] Create `useAuth` composable (login, register, token storage)
- [ ] Set up Pinia store for user state
- [ ] Implement route middleware for auth guard
- [ ] Build login and register pages
- [ ] Build project list page with API integration
- [ ] Set up `$api` fetch wrapper with JWT header injection

**Key Concepts**: Nuxt 3 composables, Pinia, route middleware, fetch

---

### Phase 4 — Frontend Features (Week 4)

**Goal**: Full Kanban board with CRUD operations

- [ ] Build Kanban board component (`KanbanBoard.vue`)
- [ ] Implement drag-to-move between columns (status update via PATCH)
- [ ] Build task creation modal with form validation
- [ ] Build task detail page with edit form + comments
- [ ] Add optimistic UI updates
- [ ] Handle loading and error states throughout

**Key Concepts**: Component design, event emitting, optimistic updates, error UX

---

### Phase 5 — Nginx & Production Prep (Week 5)

**Goal**: Everything runs behind Nginx, production-like config

- [ ] Write `nginx.conf` to proxy `/api/*` and `/*`
- [ ] Add Nuxt 3 to Docker Compose
- [ ] Add Nginx to Docker Compose
- [ ] Configure CORS policy on .NET API
- [ ] Add health check endpoint (`/api/health`)
- [ ] Add `.env` file with all secrets (`.gitignore` it)
- [ ] Write README with setup instructions

**Key Concepts**: Nginx reverse proxy, CORS, health checks, 12-factor config

---

### Phase 6 — Polish & Extras (Ongoing)

**Goal**: Strengthen skills with stretch features

- [ ] Add pagination to task and project lists
- [ ] Add search/filter tasks by keyword, status, priority
- [ ] Add due date reminders (UI indicator when overdue)
- [ ] Write unit tests for services (xUnit + Moq)
- [ ] Write integration tests for API endpoints
- [ ] Add GitHub Actions CI pipeline

---

## Key Libraries Reference

### Backend (.NET 8)

| Package | Purpose |
|---|---|
| `Microsoft.EntityFrameworkCore` | ORM |
| `Pomelo.EntityFrameworkCore.MySql` | MySQL provider for EF Core |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | JWT auth |
| `FluentValidation.AspNetCore` | Request validation |
| `Swashbuckle.AspNetCore` | Swagger / OpenAPI |
| `Serilog.AspNetCore` | Structured logging |
| `xunit` + `Moq` | Unit testing |

### Frontend (Nuxt 3)

| Package | Purpose |
|---|---|
| `@pinia/nuxt` | State management |
| `@nuxtjs/tailwindcss` | CSS utility framework |
| `zod` | Runtime type validation |
| `@vueuse/nuxt` | Utility composables |
| `dayjs` | Date formatting |

---

## Common Commands

```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f backend

# Run EF Core migrations
dotnet ef migrations add InitialCreate --project TaskFlow.Infrastructure --startup-project TaskFlow.API
dotnet ef database update --project TaskFlow.Infrastructure --startup-project TaskFlow.API

# Run backend tests
dotnet test

# Install frontend dependencies
cd frontend && npm install

# Frontend dev server (outside Docker)
cd frontend && npm run dev
```

---

## Decisions & Rationale

| Decision | Reason |
|---|---|
| Clean Architecture (4 projects) | Teaches separation of concerns; mirrors real-world .NET projects |
| Repository Pattern over raw DbContext | Abstraction for testability; industry standard |
| JWT over sessions | Stateless; works well with SPA frontends |
| Nuxt 3 over plain Vue | SSR capability + file-based routing; mirrors production setups |
| Pomelo MySQL provider | Official and best-maintained EF Core provider for MySQL |
| Docker Compose for MySQL only first | Reduces complexity in Phase 1; add other services incrementally |

---

## Glossary

| Term | Definition |
|---|---|
| DTO | Data Transfer Object — a plain class used to carry data between API layers |
| Repository | A class that abstracts database access for a given entity |
| Composable | A Nuxt/Vue function (prefix `use`) that encapsulates reusable stateful logic |
| EF Core | Entity Framework Core — .NET ORM for database interaction |
| JWT | JSON Web Token — stateless auth token sent in Authorization header |
| Kanban | A board with columns (Todo / In Progress / Done) for visualizing work |
