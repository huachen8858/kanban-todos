# TaskFlow — Todo

## Phase 1: Infrastructure Setup ✅

- [x] Create project root directory structure
- [x] Write `docker-compose.yml` with MySQL 8 service
- [x] Write `.env` file with database credentials
- [ ] Connect MySQL Workbench to Docker MySQL (port 3306) — *manual user step*
- [ ] Run schema SQL to create all tables — *deferred: handled via EF Core migrations in Phase 2*
- [x] Create .NET 8 solution with 4 projects (API / Application / Domain / Infrastructure)
- [x] Add EF Core + Pomelo MySQL package references
- [x] Configure `DbContext` and connection string from environment variable
- [x] Verify API can query MySQL (add a `/api/health` endpoint)

**Notes (Phase 1)**
- SDK on this machine is .NET 9 (not 8); all packages pinned to 9.x accordingly
- `docker-compose.yml` `version:` attribute removed warning — cosmetic only
- Swagger UI accessible at `http://localhost:5000/swagger`
- `curl http://localhost:5000/api/health` → `{"success":true,"data":{"database":true},...}`

## Phase 2: Backend CRUD ✅

- [x] Domain entities: User, Project, Task, TaskComment
- [x] EF Core migrations: `InitialCreate`
- [x] Auth: register + login endpoints with JWT
- [x] Project CRUD (5 endpoints)
- [x] Task CRUD (6 endpoints) with status PATCH
- [x] TaskComment endpoints (3 endpoints)
- [x] FluentValidation for all request DTOs
- [x] Global exception middleware returning standard error envelope
- [x] Swagger configured and accessible at `/swagger`

**Notes (Phase 2)**
- Entity named `TaskItem` (not `Task`) — avoids C# keyword conflict
- Enum `TaskStatus` disambiguated via `using TaskStatus = TaskFlow.Domain.Enums.TaskStatus`
- Validators placed in `TaskFlow.API/Validators/` (not Application) — Application cannot reference API DTOs (circular dep)
- Swashbuckle pinned to 6.9.0 — version 10.x changed OpenAPI models namespace
- JWT Bearer auth configured in Swagger UI (Authorize button available)
- Verified: register → login → create project → create task → PATCH status → validation 400 → all passing

## Phase 3: Frontend Foundation ✅

- [x] Create Nuxt 3 project with TypeScript + Tailwind
- [x] Configure `runtimeConfig` for API base URL
- [x] `useApi` axios wrapper (injects JWT, handles 401)
- [x] `useAuth` composable + Pinia auth store
- [x] Login page (`/login`)
- [x] Register page (`/register`)
- [x] Auth route middleware (redirect unauthenticated users)
- [x] Project list page (`/projects`)

**Notes (Phase 3)**
- Nuxt 4 minimal template — source dir is `app/` (not project root)
- API wrapper uses axios with request/response interceptors (not `$fetch`)
- JWT stored in SSR-safe cookies via `useCookie` (not localStorage)
- Dev proxy: Nitro `devProxy` routes `/api/*` → `http://localhost:5000/api` (no CORS issues in dev)
- Auth middleware is named (not global) — pages opt-in via `definePageMeta({ middleware: 'auth' })`
- Verified: `/login` → 200, `/projects` unauthenticated → 302 redirect, build → 0 errors

## Phase 4: Frontend Features ✅

- [x] Project detail page with Kanban board (`/projects/[id]`)
- [x] `TaskCard.vue` component
- [x] `KanbanBoard.vue` with three columns
- [x] Drag tasks between columns (PATCH status)
- [x] Create task modal
- [x] Task detail page (`/tasks/[id]`)
- [x] Task comments section
- [x] Loading skeletons and error states

**Notes (Phase 4)**
- Drag-and-drop via `vue-draggable-plus` (SortableJS wrapper for Vue 3); drag `@end` event emits `statusChange` → PATCH `/tasks/{id}/status`
- `KanbanBoard.vue` uses `watch` to sync parent `tasks` prop into per-column reactive arrays; updating spinner shown per-task during PATCH
- `CreateTaskModal.vue` emits `@created` + triggers `fetchTasks` re-fetch on success (clean state)
- `TaskCard.vue` shows priority badge (color-coded), overdue date in red, NuxtLink to detail page
- Task detail page (`/tasks/[id]`) shows full info + comments with add/delete; delete only shown for own comments
- Loading skeletons via Tailwind `animate-pulse` (no extra library)
- Verified: build → 0 errors; `/login` → 200; `/projects/1` unauthenticated → 302; `/tasks/1` → 302

## Phase 5: Nginx & Docker Compose Full Stack ✅

- [x] Write `nginx/nginx.conf`
- [x] Add Nuxt frontend service to `docker-compose.yml`
- [x] Add Nginx service to `docker-compose.yml`
- [x] Add .NET backend service to `docker-compose.yml`
- [x] Configure CORS on .NET API
- [x] Write README.md with setup instructions
- [ ] End-to-end test: `docker-compose up --build` — *manual verification step*

**Notes (Phase 5)**
- Backend Dockerfile: multi-stage `sdk:9.0` → `aspnet:9.0`; context is `./backend`
- Frontend Dockerfile: multi-stage `node:20-alpine`; runs `.output/server/index.mjs` (Nuxt 4 standalone)
- .NET 9 Docker default port is 8080 → override with `ASPNETCORE_HTTP_PORTS=5000`
- DB connection in Docker via `ConnectionStrings__DefaultConnection` env var (overrides appsettings)
- JWT config injected via `Jwt__Secret`, `Jwt__Issuer`, `Jwt__Audience` env vars
- Nginx routes: `/api/` → `backend:5000`, `/swagger` → `backend:5000`, `/` → `frontend:3000`
- CORS policy `AllowFrontend` added (permits `http://localhost`, any method/header)
- `DB_CONNECTION_STRING` removed from `.env` (docker-compose injects connection string directly)
- Start: `docker-compose up --build` → browse `http://localhost`

## Phase 6: Polish (Ongoing)

- [ ] Pagination on task lists
- [ ] Search and filter tasks
- [ ] Overdue task indicators
- [ ] Unit tests for services (xUnit + Moq)
- [ ] API integration tests
- [ ] GitHub Actions CI
