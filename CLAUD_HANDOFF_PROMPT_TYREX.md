# Claud Handoff Prompt — TYREX MVP Continuation

Use this prompt as the operating brief for continuing the current TYREX project.

---

## Prompt to give Copilot

You are taking over an existing in-progress project.

Read the repository carefully before making changes.
Do not restart from scratch.
Do not generate a superficial setup.
Do not stop at scaffolding.
Your mission is to **continue and complete the MVP implementation** of the TYREX automotive workshop platform based on the existing codebase, the business context, and the backlog.

### Mandatory first step
Read these files fully and treat them as the source of truth:
- `PROJECT_CONTEXT.md`
- `TASKS.md`

Then inspect the **current repository state** and identify:
1. what has already been implemented
2. what is only scaffolded / placeholder-level
3. what is missing compared to the target MVP
4. what is incorrectly designed compared to the intended architecture

You must continue from the current project state and evolve it into a real MVP.

---

## Project goal
Build a real MVP for an automotive workshop management platform covering the end-to-end workshop workflow:
- Reception / vehicle intake
- Repair order (OR) creation
- Diagnostic & quote
- Quote follow-up and approval
- Parts / inventory / supplier order basics
- Repair tracking
- Quality control
- Billing / vehicle delivery
- Users / roles / permissions
- Notifications and document generation at MVP level

---

## Required tech direction
- Backend: ASP.NET Core Web API
- Architecture: Clean Architecture + DDD + modular monolith
- Frontend: React + TypeScript
- Database: PostgreSQL
- ORM: Entity Framework Core
- Auth: JWT + refresh tokens
- Authorization: roles + permissions
- Validation: FluentValidation
- API docs: Swagger/OpenAPI
- Tests: unit + integration + key frontend tests when relevant

---

## Critical instruction
The previous AI may have produced a basic setup only.
You must **audit the current project honestly**.
If something is weak, incomplete, or only placeholder-level, say so explicitly and fix it.

Do not pretend the project is more advanced than it is.
Do not keep fake structure without real implementation.
Do not keep empty architectural folders unless they are immediately useful.

---

## What you must do now

### Phase A — Repository audit
Inspect the existing codebase and produce a brief but precise audit with these sections:
1. Current backend structure
2. Current frontend structure
3. Current implemented modules
4. Current authentication / authorization state
5. Current database/migrations state
6. Current tests state
7. Gaps vs PROJECT_CONTEXT.md and TASKS.md
8. Technical debt / weak design decisions to correct

### Phase B — Target architecture confirmation
After the audit, confirm or refine the target architecture:
- bounded contexts / modules
- solution structure
- folder structure
- aggregates/entities/value objects
- application use cases
- API route map
- frontend page/route map
- database schema direction
- security model
- testing strategy

Only refine it if needed, but keep it aligned with the project context and MVP scope.

### Phase C — Continue implementation
Then implement the missing MVP pieces in a practical order.
You must prioritize real business flow over cosmetic work.

Recommended order unless the repo state suggests a better one:
1. Foundation hardening
   - auth
   - roles/permissions
   - base abstractions
   - audit fields
   - error handling
   - validation pipeline
   - API conventions
   - logging
   - health checks
   - Swagger
2. Reception / OR creation
3. Diagnostic & quote
4. Quote approval flow
5. Inventory / parts basics
6. Repair tracking
7. Quality control
8. Billing / delivery
9. Notifications / PDFs / documents
10. Reporting basics required for MVP
11. Tests + bug fixes + end-to-end validation

---

## Backend expectations
Use a strong modular monolith structure. Prefer something like:

- `src/BuildingBlocks`
- `src/Modules/Identity`
- `src/Modules/Reception`
- `src/Modules/Diagnostics`
- `src/Modules/Quotes`
- `src/Modules/Inventory`
- `src/Modules/Repairs`
- `src/Modules/Quality`
- `src/Modules/Billing`
- `src/Modules/Notifications`
- `tests/`

Each module should separate, where appropriate:
- Domain
- Application
- Infrastructure
- API contracts/endpoints

Keep the domain clean.
Keep orchestration in application layer.
Use EF Core mappings properly.
Use migrations properly.
Avoid anemic naming and vague classes.

---

## Frontend expectations
The UI must behave like a real workshop operations tool, not a generic demo admin.

Expected MVP screens include:
- Login
- Dashboard
- Repair orders list/detail
- Vehicle intake / reception form
- Diagnostic form
- Quote review / send / approval tracking
- Stock / parts / supplier order basics
- Repair tracking board
- Quality checklist
- Billing / delivery screen
- Role-aware navigation and guards

Use feature-based organization.
Use a clean API client layer.
Use clear status labels and business naming.

---

## Quality expectations
For every important feature:
- implement real domain/application logic
- connect backend and frontend properly
- add validation
- add tests for happy path and key failure paths
- keep code production-minded

Where a workflow is incomplete, complete it.
Where a screen is only static, connect it.
Where a module is only placeholder, implement it.

---

## Rules for working on this repo
- Do not ask broad vague questions.
- Only ask a question if absolutely blocking.
- Otherwise make reasonable assumptions and document them.
- Use English for code, APIs, folders, classes, and database objects.
- Keep comments useful and minimal.
- Do not leave fake TODOs.
- Prefer real implementation over decorative setup.
- Preserve continuity with the existing repo where it is good.
- Refactor where needed if current code quality is too weak.

---

## Required output format in your next response
In your next response, do all of the following:

1. **Repository audit**
   - What exists
   - What is missing
   - What is weak

2. **Gap analysis vs source files**
   - Compare current repo with `PROJECT_CONTEXT.md` and `TASKS.md`

3. **Implementation plan from current state**
   - Exact next steps in priority order

4. **Start coding immediately**
   - Generate or modify real files
   - Do not stop at explanation

5. **Validation actions**
   - Run/build/test instructions
   - Mention what should be verified next

---

## Explicit success criteria
You are successful only if the repo moves from basic setup/scaffold to a real usable MVP implementation.

That means:
- working auth and authorization
- real business modules
- persisted data model
- connected frontend pages
- valid APIs
- migrations
- tests
- end-to-end usable workflow for MVP

---

## First concrete task
Start now by:
1. reading `PROJECT_CONTEXT.md` and `TASKS.md`
2. auditing the current repository
3. identifying the delta between current code and expected MVP
4. generating the first real implementation batch needed to move the project forward

Do not restart. Continue intelligently from the existing project.

