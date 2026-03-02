# Lessons Learned

> Updated after every correction or mistake. Review at the start of each session.

## Format

```
[DATE] — [CATEGORY] — [WHAT WENT WRONG] → [RULE TO PREVENT IT]
```

---

## Lessons

[2026-03-01] — .NET NAMING — `TaskStatus` clashes with `System.Threading.Tasks.TaskStatus` when implicit usings are enabled → Use `using TaskStatus = TaskFlow.Domain.Enums.TaskStatus;` alias at file top to disambiguate.

[2026-03-01] — ARCHITECTURE — Placed FluentValidation validators in `Application/Validators/` but Application cannot reference API DTOs (circular dep: API → Application → API) → Put validators in `TaskFlow.API/Validators/` where DTOs live; Application layer should not know about API contracts.

[2026-03-01] — NUGET VERSIONS — `Swashbuckle.AspNetCore` 10.x changed the `Microsoft.OpenApi.Models` namespace, breaking `OpenApiSecurityScheme` references → Pin Swashbuckle to 6.9.0 for .NET 9 projects; do not blindly take the latest Swashbuckle version.

[2026-03-01] — SHELL — Bash tool CWD resets to project root (`/Users/hua/Documents/kanban-todos`) between sessions; always use `backend/` prefix for dotnet commands when CWD is the project root.

[2026-03-01] — TESTING — Only 5/14 endpoints were tested after Phase 2 implementation — Phase considered complete without testing GET/PUT/DELETE/comments → Always run a full endpoint matrix test (all verbs + 401/403/404 error paths) before marking a phase done.

[2026-03-01] — ASP.NET CORE — `[ApiController]` attribute auto-returns its own 400 format for model binding failures, bypassing GlobalExceptionMiddleware → Always add `SuppressModelStateInvalidFilter = true` in `ConfigureApiBehaviorOptions` to ensure consistent ApiResponse envelope.

[2026-03-01] — SHELL — Bash `!` in double-quoted strings triggers shell history expansion and corrupts JSON → Use single quotes `'...'` for curl `-d` JSON payloads to avoid shell expansion.

---

## Common .NET Pitfalls to Avoid

- Always register services in `Program.cs` before using them (`builder.Services.Add...`)
- EF Core: call `await _context.SaveChangesAsync()` after mutations, not `SaveChanges()`
- Never expose `DbContext` directly from controllers — use repository/service layer
- `async` all the way down — avoid `.Result` or `.Wait()` which can deadlock in ASP.NET

## Common Nuxt 3 Pitfalls to Avoid

- `useFetch` is server-side by default; use `$fetch` inside event handlers
- Always `await` composable async functions before reading reactive state
- Do not call Pinia stores outside of `setup()` context (use `useStore()` pattern)
- Nuxt auto-imports work only for files in `components/`, `composables/`, `utils/`

## Common Docker Pitfalls to Avoid

- MySQL takes ~5-10 seconds to be ready; use `healthcheck` + `depends_on` with `condition: service_healthy`
- Environment variables in `docker-compose.yml` must reference `.env` — never hardcode secrets
- After changing `nginx.conf`, always `docker-compose restart nginx`
