# TYREX — PROJECT_CONTEXT.md

## 1. Project identity

- **Project name:** TYREX Atelier Automobile Platform
- **Context:** Digitalization of the automotive repair workshop process for Bosch Car Service / Repar Expert
- **Goal:** Replace fragmented paper-based and verbal processes with a structured digital workflow from vehicle reception to restitution
- **Target version:** MVP V1
- **Frontend stack:** React + TypeScript
- **Backend stack:** ASP.NET Core Web API
- **Architecture style:** Clean Architecture + DDD + modular monolith first, with future-ready boundaries for extraction if needed

---

## 2. Product vision

TYREX is a workshop management platform that tracks the full repair lifecycle of a vehicle:

1. Reception and OR creation
2. Diagnostic and estimate generation
3. Estimate approval / refusal
4. Parts ordering and stock movement
5. Repair execution tracking
6. Quality control and closure
7. Invoicing, payment, notification, and vehicle restitution

The platform must provide:
- end-to-end traceability
- zero-paper operational flow
- role-based interfaces by workshop function
- real-time visibility for operational teams
- customer communication by digital channels
- auditable business process execution

---

## 3. Business objectives

### Primary goals
- Reduce manual re-entry and paper usage
- Improve speed of OR creation and repair processing
- Improve traceability of work, parts, and approvals
- Reduce billing omissions for parts and labor
- Improve customer transparency and communication
- Give management live operational visibility

### Success indicators for MVP
- All repair orders created digitally
- All accepted repairs tracked digitally through closure
- All consumed parts linked to ORs
- All final invoices generated from closed ORs
- Quality checklist mandatory before invoicing
- Technician time and OR statuses visible in real time

---

## 4. Roles and actors

### Internal users
- **Receptionnaire / Service Advisor**
  - Creates OR
  - Manages customer information
  - Sends estimates
  - Handles client communication
  - Final restitution and simple billing actions

- **Technician**
  - Executes diagnostic
  - Adds notes/photos/videos
  - Starts / pauses / ends interventions
  - Requests parts
  - Fills quality checklist items

- **Chef d'équipe / Workshop Lead**
  - Supervises workshop execution
  - Assigns technicians
  - Validates complex technical steps
  - Monitors delays and priorities
  - Confirms quality and road test decisions

- **Magasinier / Storekeeper**
  - Manages stock
  - Processes workshop part requests
  - Creates supplier orders
  - Receives parts into inventory
  - Issues parts to ORs
  - Handles counter sales without service

- **Cashier / Billing role**
  - Final payment collection
  - Payment method capture
  - Invoice release

- **Admin / Technical Director**
  - Full access
  - Configuration
  - User and role administration
  - Reporting oversight

### External actors
- Customer (particulier or company / en compte)
- Supplier
- External service provider / subcontractor

---

## 5. Core business vocabulary

- **OR (Ordre de Réparation):** Main repair order dossier for a vehicle
- **Estimate / Devis:** Commercial proposal generated after diagnostic
- **Accepted flow:** Repair continues after customer approval
- **Refused flow:** Dossier closes without repair, possibly with diagnostic fee
- **Service rapide:** Fast-service flow with tighter operational tracking
- **Retour technique:** Return after previous intervention
- **Sinistre:** Insurance-related repair order
- **Counter sale:** Parts sale without vehicle repair order

---

## 6. In-scope MVP V1

The MVP must cover the complete operational chain for the core workshop process.

### Functional scope in V1
1. Customer and vehicle intake
2. OR creation (4 OR types)
3. Photo capture and digital dossier
4. Diagnostic with notes, photos, and technician assignment
5. Estimate generation and estimate versioning
6. Customer approval/refusal tracking
7. Parts request, stock lookup, supplier order, reception, and issue to OR
8. Repair execution tracking (start/pause/waiting part/done)
9. Quality checklist and mandatory lead validation
10. Automatic invoice generation from closed OR
11. Payment capture
12. Notification sending (estimate ready, vehicle ready, documents)
13. Vehicle restitution and closure trace
14. Basic dashboards / operational KPI views
15. RBAC and audit trail

### Explicitly deferred to V2
- Full HR module
- Loyalty cards and customer wallet
- Full customer portal / mobile app
- Advanced supplier platform integration
- Advanced invoice recovery / collections workflows
- Advanced compensation / bonus engine
- Rich BI and executive dashboards beyond MVP essentials

---

## 7. Out-of-scope for MVP

To keep V1 realistic, the following should not block first production delivery:

- Full ERP / accounting integration
- Full CRM implementation
- Deep insurance workflow automation beyond core OR classification
- Multi-branch / multi-tenant enterprise rollout complexities
- AI-based predictive maintenance
- AI anomaly detection on operations
- Full offline sync for every role
- Complex promotions / loyalty system

---

## 8. Functional modules in MVP

## Module 1 — Reception du véhicule

### Responsibilities
- Search/create customer
- Search/create vehicle
- Capture intake information
- Create OR
- Select OR type: Sinistre / Général / Service Rapide / Retour Technique
- Capture state-of-vehicle photos
- Send OR / photos to customer if needed
- Separate internal company vehicles from external clients

### Inputs
- Customer identity
- Vehicle identity
- Visit reason
- Vehicle photos
- Communication preference

### Outputs
- OR created
- OR number
- Initial status
- Vehicle dossier with intake photos

---

## Module 2 — Diagnostic & Devis

### Responsibilities
- Assign technician
- Capture technical diagnosis
- Add notes and media
- View vehicle history
- Consult stock
- Build estimate
- Internal technical review before client-facing approval

### Inputs
- Open OR
- Vehicle history
- Technician skill matrix
- Current stock references

### Outputs
- Diagnostic record
- Estimate draft / version
- Technical approval state

---

## Module 3 — Suivi devis & approbation

### Responsibilities
- Send estimate by email / SMS / WhatsApp
- Track estimate status: pending / accepted / refused / modified
- Auto-reminders
- Preserve estimate version history
- Capture and archive client approval proof

### Outputs
- Approved estimate
- Refused estimate
- New estimate version if modified

---

## Module 4 — Commande & Réception Pièces

### Responsibilities
- Process workshop part requests
- Check stock availability
- Reserve available stock
- Create supplier order if stock unavailable
- Track ETA and delays
- Receive stock
- Issue parts to OR via scan / selection
- Counter-sale flow without vehicle repair

### Outputs
- Stock reservation
- Supplier order
- Stock movement
- OR-linked part consumption

---

## Module 5 — Suivi de Réparation

### Responsibilities
- Start / pause / resume / complete repair steps
- Track technician time
- Show live OR progression
- Raise delay alerts
- Allow workshop-lead supervision
- Flag waiting-part / waiting-decision states
- Track external intervention and road test requirements

### Outputs
- Real-time OR status timeline
- Technician time logs
- Repair progression record

---

## Module 6 — Contrôle Qualité

### Responsibilities
- Fill QC checklist on tablet
- Add post-work photos
- Add technical notes / recommendations
- Capture workshop lead digital validation
- Optionally generate quality report
- Close OR once QC is validated

### Outputs
- QC checklist
- QC signature
- Optional quality report
- OR closure trigger

---

## Module 7 — Facturation & Restitution

### Responsibilities
- Generate invoice automatically from closed OR
- Compare estimate vs invoice
- Capture payment method
- Send invoice and QC checklist digitally
- Register safe vehicle handover
- Store delivery confirmation

### Outputs
- Final invoice
- Payment record
- Delivery / restitution record
- Customer-ready documents

---

## 9. Proposed domain design (DDD)

Use a **modular monolith with bounded contexts** for MVP.

### Recommended bounded contexts
1. **Identity & Access**
   - Users
   - Roles
   - Permissions
   - Auth sessions

2. **CRM / Parties**
   - Customers
   - Company accounts
   - Contacts
   - Internal company vehicles ownership mapping

3. **Fleet / Vehicle Management**
   - Vehicles
   - Vehicle identity
   - Vehicle history
   - Vehicle media

4. **Workshop Intake**
   - Reception
   - OR creation
   - Intake photos
   - OR typing

5. **Diagnostic & Estimating**
   - Diagnostics
   - Findings
   - Estimate versions
   - Technical validations

6. **Inventory & Procurement**
   - Stock items
   - Batches / serials
   - Warehouse movements
   - Supplier orders
   - Counter sales

7. **Repair Execution**
   - Assignment
   - Work logs
   - Status transitions
   - Delay monitoring
   - External interventions
   - Road tests

8. **Quality**
   - QC checklist templates
   - QC checklist instances
   - QC validations
   - Quality report generation

9. **Billing & Payments**
   - Invoice
   - Invoice lines
   - Payment records
   - Refund / adjustment placeholder

10. **Notifications & Documents**
   - Email/SMS/WhatsApp jobs
   - PDF generation
   - Delivery tracking
   - Message templates

11. **Reporting**
   - Read models
   - KPIs
   - Dashboard projections

### Why modular monolith first
- Faster delivery than microservices
- Easier transaction consistency across modules
- Easier testing and deployment in MVP
- Still scalable if module boundaries are respected

---

## 10. Recommended backend architecture

### Style
- ASP.NET Core Web API
- Clean Architecture
- DDD tactical patterns where justified
- CQRS for application use cases
- Domain events for important business transitions
- Repository pattern only at aggregate boundaries
- Transactional consistency per use case

### Solution layout

```text
src/
  Tyrex.Api/
  Tyrex.Application/
  Tyrex.Domain/
  Tyrex.Infrastructure/
  Tyrex.Contracts/
  Tyrex.SharedKernel/

 tests/
  Tyrex.UnitTests/
  Tyrex.ApplicationTests/
  Tyrex.IntegrationTests/
  Tyrex.ArchitectureTests/
```

### Layer responsibilities

#### Tyrex.Domain
- Entities
- Value objects
- Aggregates
- Domain services
- Domain events
- Business invariants

#### Tyrex.Application
- Commands / Queries
- Handlers
- DTOs
- Use-case orchestration
- Interfaces for persistence / notifications / files
- Validation

#### Tyrex.Infrastructure
- EF Core persistence
- Auth implementation
- SMS / Email / WhatsApp providers
- File storage
- PDF generation
- OCR / scan integration abstractions

#### Tyrex.Api
- REST endpoints
- Auth middleware
- OpenAPI / Swagger
- API filters / exception mapping

#### Tyrex.Contracts
- Request/response contracts
- Integration contracts

#### Tyrex.SharedKernel
- Base abstractions
- Result type
- Domain primitives
- Audit interfaces

---

## 11. Recommended frontend architecture

### Stack
- React + TypeScript
- Feature-based folder structure
- Server state with TanStack Query
- Form handling with strong validation
- UI library with reusable design system
- Routing by role-aware layouts

### Frontend app zones
- Reception app view
- Technician tablet view
- Workshop lead dashboard
- Magasinier back-office view
- Billing / restitution view
- Admin area
- Reporting area

### Suggested front structure

```text
src/
  app/
  modules/
    auth/
    customers/
    vehicles/
    repair-orders/
    diagnostics/
    estimates/
    inventory/
    procurement/
    repairs/
    quality/
    billing/
    notifications/
    reporting/
  shared/
  ui/
```

---

## 12. Core aggregate candidates

### RepairOrder aggregate
Owns:
- OR number
- type
- customer/vehicle links
- intake state
- lifecycle status
- closure state

### Diagnostic aggregate
Owns:
- findings
- notes
- media references
- assigned technician
- technical validation state

### Estimate aggregate
Owns:
- versions
- line items
- approval status
- client approval proof

### StockItem / Inventory aggregate
Owns:
- part identity
- stock state
- batch/serial data
- reservation state

### SupplierOrder aggregate
Owns:
- supplier lines
- ETA
- reception state

### RepairExecution aggregate
Owns:
- work logs
- time tracking
- pause reasons
- external work / road test flags

### QualityChecklist aggregate
Owns:
- checklist items
- evidence
- lead validation

### Invoice aggregate
Owns:
- invoice lines
- totals
- payment status
- delivery linkage

---

## 13. Key business workflows

## Workflow A — Accepted repair
1. Receive vehicle
2. Create OR
3. Diagnose
4. Generate estimate
5. Customer approves
6. Reserve/order parts
7. Execute repair
8. Perform quality control
9. Auto-generate invoice
10. Collect payment / register en-compte state
11. Notify customer
12. Deliver vehicle

## Workflow B — Refused estimate
1. Receive vehicle
2. Create OR
3. Diagnose
4. Generate estimate
5. Customer refuses
6. Close dossier
7. Optionally invoice diagnostic fee
8. Notify customer
9. Deliver vehicle unrepaired

## Workflow C — Counter sale
1. Search/create customer
2. Create counter sale order
3. Reserve/issue part from stock
4. Invoice immediately
5. Deliver part

---

## 14. Recommended OR statuses

Use explicit status transitions rather than free text.

### Intake / commercial
- Draft
- Open
- AwaitingDiagnostic
- Diagnosing
- EstimateReady
- AwaitingCustomerApproval
- EstimateApproved
- EstimateRefused

### Logistics / workshop
- AwaitingParts
- PartsReserved
- InRepair
- Paused
- WaitingPart
- WaitingLeadDecision
- ExternalService
- RoadTestPending
- RepairCompleted

### Quality / billing / closure
- QualityPending
- QualityValidated
- Invoiced
- Paid
- ReadyForDelivery
- Delivered
- Closed
- ClosedUnrepaired

---

## 15. API design guidance

### API style
- REST for MVP
- Strong resource naming
- Separation between commands and queries in application layer

### Examples of API modules
- `/api/auth`
- `/api/customers`
- `/api/vehicles`
- `/api/repair-orders`
- `/api/diagnostics`
- `/api/estimates`
- `/api/inventory`
- `/api/supplier-orders`
- `/api/repairs`
- `/api/quality-checks`
- `/api/invoices`
- `/api/payments`
- `/api/notifications`
- `/api/reports`

### Example use cases
- Create repair order
- Add intake photos
- Assign technician
- Submit diagnosis
- Generate estimate version
- Approve / refuse estimate
- Request part for OR
- Receive supplier order
- Issue part to OR
- Start / pause / complete repair
- Validate QC checklist
- Generate invoice
- Register payment
- Confirm vehicle delivery

---

## 16. Data persistence guidance

### Recommended database
- Relational database first
- SQL Server or PostgreSQL are both valid options
- EF Core as ORM

### Data concerns
- Strong audit trail on status changes
- Soft delete only where business-safe
- Media storage externalized from relational tables
- Concurrency handling for stock movements and OR status transitions
- Row-level history for estimate versions and payment state

### Important persistence needs
- OR numbering strategy
- Sequential audit entries
- Media metadata storage
- Document generation metadata
- Notification delivery history

---

## 17. Documents and files

The system will need document generation and archival for:
- OR summary
- Estimate PDF
- Customer approval proof
- QC checklist PDF
- Quality report PDF
- Invoice PDF
- Delivery confirmation

Media support:
- Intake photos
- Diagnostic photos/videos
- Post-repair photos

---

## 18. Security and access control

### Security goals
- Enforce role-based access by module and action
- Maintain auditability of all sensitive changes
- Protect customer and vehicle data
- Prevent invoice and stock manipulation without trace

### Access strategy
- JWT or secure token-based auth for web clients
- Refresh-token flow for session continuity
- Role + permission matrix
- Fine-grained authorization by action
- Audit logs on create/update/approve/close/payment actions

### High-value audit actions
- OR creation/edit
- Estimate creation/version change
- Estimate approval/refusal
- Stock issue/receipt
- Repair state transition
- QC validation
- Invoice generation
- Payment registration
- Delivery confirmation

---

## 19. Non-functional requirements for MVP

### Performance
- Fast reception flow on tablet
- Fast OR dashboard loading
- Inventory movements must feel immediate

### Reliability
- No duplicate OR creation
- No duplicate stock issue
- No invoice generation without closed OR

### Scalability
- Modular codebase
- Clear bounded contexts
- Async notification processing
- Background jobs for reminders and documents

### Maintainability
- Clean Architecture boundaries enforced with tests
- High unit test coverage on domain rules
- Integration tests for core business flows

### Observability
- Structured logs
- Error tracking
- Audit events
- Health checks

---

## 20. Testing strategy

### Unit tests
- Domain invariants
- Status transition rules
- Estimate approval rules
- Stock movement rules
- QC closure rules
- Invoice generation rules

### Integration tests
- Create OR end-to-end
- Diagnostic to estimate flow
- Approved estimate to parts to repair flow
- Refused estimate closure flow
- QC-to-invoice flow
- Counter-sale flow

### Architecture tests
- Domain does not depend on Infrastructure
- Application does not depend on Api
- Modules respect boundaries

### UI tests
- Reception flow
- Technician workflow
- Magasinier stock issue flow
- Billing/restitution flow

---

## 21. AI opportunities in this project

AI should support operations without overcomplicating MVP.

### Good AI use cases for V1 / V1.1
- OCR for customer/vehicle data capture from documents
- Suggested technician assignment scoring
- Notification text generation templates
- Photo classification assistance (future enhancement)

### AI use cases for later phases
- Predictive delays on OR completion
- Demand forecasting for parts
- Suggested upsell / maintenance recommendations
- Operational anomaly detection

---

## 22. Delivery strategy recommendation

### Recommended implementation strategy
- Build **modular monolith** first
- Release by workflow completeness, not by isolated screens only
- Prioritize all core modules in thin but usable slices
- Keep V1 focused on one branch / one workshop model first

### Suggested release philosophy
- V1 must allow a real workshop team to process a vehicle from intake to delivery
- Missing advanced BI or advanced integrations must not block core operations
- Every action should leave a trace

---

## 23. Major open questions to resolve before development

These must be clarified with stakeholders before or during Sprint 0:

1. Exact rules for company / en-compte customers
2. Diagnostic fee rules and when they apply
3. Whether partial estimate approval is allowed
4. Mandatory vs optional client signatures at intake and delivery
5. Invoice/payment rules for company accounts
6. Exact supplier order validation workflow
7. Counter-sale invoice format and tax rules
8. Exact QC templates per OR type
9. Road-test recording policy
10. External intervention cost capture rules
11. Notification providers and channels for production
12. Final reporting metrics for MVP dashboard

---

## 24. Recommended MVP architecture decision summary

### Chosen technical direction
- **Backend:** ASP.NET Core Web API
- **Frontend:** React + TypeScript
- **Architecture:** Clean Architecture + DDD + modular monolith
- **Persistence:** relational database + EF Core
- **Auth:** token-based auth with RBAC
- **Async work:** background jobs for notifications and document generation
- **Deployment:** container-friendly, cloud-ready

### Why this is the right fit
- Strong modularity for many business modules
- High testability and maintainability
- Good fit for transactional business workflows
- Easier long-term evolution than a tightly coupled CRUD system
- Safer foundation for future scaling and integrations

