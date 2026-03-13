# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

TYREX is an automotive repair workshop management platform (atelier automobile) for Bosch Car Service. It digitizes the complete repair lifecycle from vehicle reception to restitution.

**Stack:**
- **Backend:** ASP.NET Core 9 Web API with Clean Architecture + CQRS + MediatR
- **Frontend:** React 19 + TypeScript + Vite
- **Database:** SQLite (MVP), with PostgreSQL support configured for production
- **ORM:** Entity Framework Core 9
- **Testing:** xUnit

## Project Structure

```
Tyrex/
├── src/
│   ├── Tyrex.Api/              # REST API layer (Controllers, middleware, DI composition)
│   ├── Tyrex.Application/      # Use cases: Commands, Queries, Handlers, Validation, DTOs
│   ├── Tyrex.Domain/           # Domain entities, aggregates, value objects, domain events
│   ├── Tyrex.Infrastructure/   # EF Core persistence, repositories, auth, external services
│   ├── Tyrex.Contracts/        # Request/response contracts for API
│   └── Tyrex.SharedKernel/     # Base abstractions, Result type, common primitives
├── tests/
│   ├── Tyrex.UnitTests/
│   ├── Tyrex.ApplicationTests/
│   ├── Tyrex.IntegrationTests/
│   └── Tyrex.ArchitectureTests/
└── Tyrex.sln

tyrex-frontend/
├── src/
│   ├── api/           # API client functions
│   ├── components/    # Reusable UI components (AppLayout, ProtectedRoute)
│   ├── context/       # React context (auth)
│   ├── pages/         # Page components (Login, Dashboard, Reception, Technician, etc.)
│   └── assets/        # Static assets
├── package.json
└── vite.config.ts
```

## Architecture

**Clean Architecture with DDD patterns:**
- **Domain layer:** No external dependencies; contains business logic, entities, aggregates
- **Application layer:** Orchestrates use cases via CQRS with MediatR; defines repository interfaces
- **Infrastructure layer:** Implements persistence, auth, external services
- **Api layer:** REST endpoints, DI composition, middleware

**Dependency direction:** Domain → Application ← Infrastructure, Api

**CQRS Pattern:**
- Commands modify state (CreateRepairOrderCommand, GenerateInvoiceCommand)
- Queries read state (GetDashboardQuery)
- Each use case has: Command/Query → Handler → Validator (FluentValidation)

**MediatR Pipeline Behaviors:**
- `ValidationBehavior` - Runs FluentValidation before handlers
- `LoggingBehavior` - Logs request execution

**Result Pattern:**
- Operations return `Result<T>` or `Result` from SharedKernel
- Controllers check `result.IsFailure` to return appropriate HTTP status codes

## Common Commands

### Backend (Tyrex/)

```bash
# Build solution
cd Tyrex && dotnet build

# Run API (from Tyrex.Api directory)
cd Tyrex/src/Tyrex.Api && dotnet run

# Run all tests
cd Tyrex && dotnet test

# Run specific test project
dotnet test tests/Tyrex.UnitTests
dotnet test tests/Tyrex.ApplicationTests
dotnet test tests/Tyrex.IntegrationTests

# Run specific test
dotnet test --filter "FullyQualifiedName~TestClassName"

# Database migrations (EF Core)
cd Tyrex/src/Tyrex.Api
dotnet ef migrations add MigrationName --project ../Tyrex.Infrastructure
dotnet ef database update --project ../Tyrex.Infrastructure
```

**API runs on:** http://localhost:5101 (configured in `Properties/launchSettings.json`)

### Frontend (tyrex-frontend/)

```bash
# Install dependencies (already done)
npm install

# Development server with HMR
npm run dev

# Build for production
npm run build

# Lint
npm run lint

# Preview production build
npm run preview
```

**Frontend runs on:** http://localhost:5173 (configured for CORS in API)

## Key Patterns

### Adding a New API Endpoint

1. **Create Command/Query** in `Tyrex.Application/[Feature]/Commands/` or `Queries/`
2. **Create Handler** implementing `IRequestHandler<TCommand, Result<TResponse>>`
3. **Create Validator** using FluentValidation (automatically registered)
4. **Add Controller** in `Tyrex.Api/Controllers/` using `ISender` to dispatch

Example:
```csharp
// Command
public record CreateRepairOrderCommand(string CustomerName, string VehicleVin) : IRequest<Result<Guid>>;

// Handler
public class CreateRepairOrderCommandHandler : IRequestHandler<CreateRepairOrderCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateRepairOrderCommand request, CancellationToken ct)
    {
        // Implementation
    }
}

// Controller
[ApiController]
[Route("api/[controller]")]
public class RepairOrdersController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateRepairOrderCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.IsFailure ? BadRequest(result.Error) : Ok(result.Value);
    }
}
```

### Repository Pattern

- Interfaces defined in `Tyrex.Application/[Feature]/Interfaces/`
- Implementations in `Tyrex.Infrastructure/Persistence/Repositories/`
- Registered in `Tyrex.Infrastructure/DependencyInjection.cs`

## Configuration

**Backend:**
- `appsettings.Development.json` - SQLite database, JWT settings for local dev
- Database is auto-created on startup (`EnsureCreated()` in Program.cs)

**Frontend:**
- API base URL configured in `src/api/` files
- Uses axios for HTTP requests

## Testing

**Test Project Purposes:**
- `Tyrex.UnitTests` - Domain logic, value objects, pure functions
- `Tyrex.ApplicationTests` - Command/query handlers, validation logic
- `Tyrex.IntegrationTests` - End-to-end business flows, database integration
- `Tyrex.ArchitectureTests` - Dependency rules, layer boundaries

## Domain Concepts

**Core Aggregates:**
- **RepairOrder (OR)** - Central repair order dossier (types: Sinistre, Général, Service Rapide, Retour Technique)
- **Diagnostic** - Technical findings with media
- **Estimate** - Commercial proposals with versioning
- **StockItem/Inventory** - Parts management with reservations
- **RepairExecution** - Work logs, time tracking
- **QualityChecklist** - QC validation before closure
- **Invoice** - Auto-generated from closed ORs

**OR Status Flow:**
Draft → Open → AwaitingDiagnostic → Diagnosing → EstimateReady → AwaitingCustomerApproval → EstimateApproved → AwaitingParts → InRepair → RepairCompleted → QualityPending → QualityValidated → Invoiced → Paid → Delivered → Closed

## Development Notes

- MVP uses SQLite for simplicity; PostgreSQL packages are included for production
- JWT authentication is implemented; token configured in appsettings
- Mock services for Email and PDF generation in MVP (see `MockEmailService`, `MockPdfService`)
- CORS configured for frontend at `http://localhost:5173`
- Global exception handler converts exceptions to ProblemDetails
