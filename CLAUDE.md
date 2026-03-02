# CLAUDE.md — Project Guidelines

## Project Overview

**TaskFlow** — A full-stack task management system built with .NET 8 Web API, Nuxt 3, MySQL 8, Nginx, and Docker.

## Tech Stack

| Layer | Technology |
|---|---|
| Backend API | .NET 8 (C#), ASP.NET Core Web API |
| Frontend | Nuxt 3, TypeScript, Tailwind CSS |
| Database | MySQL 8 (Docker) |
| Reverse Proxy | Nginx |
| Containerization | Docker / Docker Compose |

---

## Code Standards

### General Rules

1. **No Chinese in code** — All code comments, variable names, function names, and file names must be in English. Chinese is allowed only in documentation (`.md` files).
2. **No magic numbers** — Use named constants or configuration values.
3. **No commented-out code** — Delete unused code; use version control instead.
4. **Explicit over implicit** — Prefer clarity over cleverness.

### C# / .NET

```csharp
// Naming conventions
public class TaskService { }           // PascalCase for classes
public async Task<TaskDto> GetById()   // PascalCase for methods
private readonly ITaskRepository _repo; // _camelCase for private fields
int taskCount = 0;                      // camelCase for local variables

// Always use async/await for I/O
public async Task<IActionResult> GetTasks()
{
    var tasks = await _taskService.GetAllAsync();
    return Ok(tasks);
}

// Use record types for DTOs
public record CreateTaskRequest(string Title, string Description, int Priority);
public record TaskResponse(int Id, string Title, bool IsCompleted, DateTime CreatedAt);
```

- Use **Repository Pattern** for data access
- Use **DTOs** for API contracts (never expose domain entities directly)
- Use **FluentValidation** for request validation
- Use **Result<T>** pattern for service layer returns when error handling is needed
- All controllers must have `[ApiController]` and route attributes
- Use **Serilog** for structured logging

### TypeScript / Nuxt 3

```typescript
// Strict typing — no `any`
interface Task {
  id: number
  title: string
  isCompleted: boolean
  createdAt: string
}

// Composables for business logic
const { tasks, fetchTasks } = useTasks()

// Always use defineProps with types
const props = defineProps<{ task: Task }>()
```

- Use **Composition API** with `<script setup lang="ts">`
- All API calls via composables (e.g., `useTasks()`, `useAuth()`)
- Use **Pinia** for global state
- Use **zod** for runtime validation of API responses

---

## Workflow Orchestration

### 1. Plan Node Default

- Enter plan mode for ANY non-trivial task (3+ steps or architectural decisions)
- If something goes sideways, STOP and re-plan immediately — do not keep pushing
- Use plan mode for verification steps, not just building
- Write detailed specs upfront to reduce ambiguity

### 2. Subagent Strategy

- Use subagents liberally to keep main context window clean
- Offload research, exploration, and parallel analysis to subagents
- For complex problems, throw more compute at it via subagents
- One task per subagent for focused execution

### 3. Self-Improvement Loop

- After ANY correction from the user: update `tasks/lessons.md` with the pattern
- Write rules for yourself that prevent the same mistake
- Ruthlessly iterate on these lessons until mistake rate drops
- Review lessons at session start for relevant project

### 4. Verification Before Done

- Never mark a task complete without proving it works
- Diff behavior between main and your changes when relevant
- Ask yourself: "Would a staff engineer approve this?"
- Run tests, check logs, demonstrate correctness

### 5. Demand Elegance (Balanced)

- For non-trivial changes: pause and ask "is there a more elegant way?"
- If a fix feels hacky: "Knowing everything I know now, implement the elegant solution"
- Skip this for simple, obvious fixes — do not over-engineer
- Challenge your own work before presenting it

### 6. Autonomous Bug Fixing

- When given a bug report: just fix it. Do not ask for hand-holding
- Point at logs, errors, failing tests — then resolve them
- Zero context switching required from the user
- Go fix failing CI tests without being told how

---

## Task Management

1. **Plan First**: Write plan to `tasks/todo.md` with checkable items
2. **Verify Plan**: Check in before starting implementation
3. **Track Progress**: Mark items complete as you go
4. **Explain Changes**: High-level summary at each step
5. **Document Results**: Add review section to `tasks/todo.md`
6. **Capture Lessons**: Update `tasks/lessons.md` after corrections

---

## Core Principles

- **Simplicity First**: Make every change as simple as possible. Minimal impact on code.
- **No Laziness**: Find root causes. No temporary fixes. Senior developer standards.
- **Minimal Impact**: Changes should only touch what is necessary. Avoid introducing bugs.

---

## Project Structure

```
taskflow/
├── backend/
│   ├── TaskFlow.API/           # ASP.NET Core Web API
│   │   ├── Controllers/
│   │   ├── DTOs/
│   │   ├── Middleware/
│   │   └── Program.cs
│   ├── TaskFlow.Application/   # Business logic, services, interfaces
│   │   ├── Services/
│   │   ├── Interfaces/
│   │   └── Validators/
│   ├── TaskFlow.Domain/        # Domain entities
│   │   └── Entities/
│   ├── TaskFlow.Infrastructure/ # Data access, EF Core
│   │   ├── Repositories/
│   │   ├── Data/
│   │   └── Migrations/
│   └── TaskFlow.Tests/         # xUnit tests
├── frontend/                   # Nuxt 3 app
│   ├── components/
│   ├── composables/
│   ├── pages/
│   ├── stores/
│   └── types/
├── nginx/
│   └── nginx.conf
├── tasks/
│   ├── todo.md
│   └── lessons.md
├── docker-compose.yml
└── CLAUDE.md
```

---

## API Conventions

- Base URL: `/api/v1/`
- Response format: always JSON
- Use **HTTP status codes** correctly:
  - `200 OK` — successful GET / PUT
  - `201 Created` — successful POST
  - `204 No Content` — successful DELETE
  - `400 Bad Request` — validation error
  - `404 Not Found` — resource not found
  - `500 Internal Server Error` — unhandled exception

### Standard Response Envelope

```json
{
  "success": true,
  "data": { },
  "message": "Operation successful",
  "errors": []
}
```

---

## Docker & Infrastructure

- All services run via `docker-compose up`
- Environment variables via `.env` file (never commit secrets)
- MySQL data persisted via named Docker volume
- Nginx proxies:
  - `/api/*` → backend (port 5000)
  - `/*` → frontend (port 3000)

---

## Git Conventions

```
feat: add task completion toggle
fix: resolve null reference in task service
refactor: extract task validation to validator class
docs: update API endpoint documentation
chore: upgrade EF Core to 8.0.5
```

---

## Environment Variables

```env
# .env (never commit to git)
MYSQL_ROOT_PASSWORD=rootpassword
MYSQL_DATABASE=taskflow
MYSQL_USER=taskflow_user
MYSQL_PASSWORD=taskflow_pass
DB_CONNECTION_STRING=Server=mysql;Database=taskflow;Uid=taskflow_user;Pwd=taskflow_pass;

ASPNETCORE_ENVIRONMENT=Development
JWT_SECRET=your-256-bit-secret-here

NUXT_PUBLIC_API_BASE=http://localhost/api/v1
```
