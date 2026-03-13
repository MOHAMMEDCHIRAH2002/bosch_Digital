# TYREX — TASKS.md (MVP V1)

## 1. Delivery objective

Deliver a first production-capable MVP that supports the full core workshop journey:

- reception
- OR creation
- diagnostic
- estimate approval/refusal
- parts management
- repair tracking
- quality control
- invoicing
- payment
- restitution
- core dashboards

The MVP must be usable by a real workshop team in one operational context.

---

## 2. Delivery principles

- Build thin vertical slices across the full workflow
- Do not overbuild V2 features in MVP
- Keep architecture clean from day one
- Prioritize auditability, correctness, and stock/invoice integrity
- Ship internal admin and operational interfaces before advanced customer experience features

---

## 3. MVP tracks

- Track A — Product & Functional Framing
- Track B — Backend Foundation
- Track C — Frontend Foundation
- Track D — Security & RBAC
- Track E — Core Business Modules (1 to 7)
- Track F — Notifications & Documents
- Track G — Reporting & Dashboards
- Track H — Testing & Quality
- Track I — DevOps & Observability
- Track J — Go-live Readiness

---

## 4. Sprint 0 — Product framing and architecture

### P0.1 Functional framing
- [ ] Consolidate MVP business scope
- [ ] Confirm V1 vs V2 boundary
- [ ] Confirm OR types and business rules
- [ ] Confirm accepted/refused estimate flows
- [ ] Confirm roles and permissions matrix
- [ ] Confirm company/en-compte customer rules
- [ ] Confirm diagnostic fee rule
- [ ] Confirm payment and restitution rules
- [ ] Confirm counter-sale flow

### P0.2 Domain framing
- [ ] Define bounded contexts
- [ ] Define ubiquitous language glossary
- [ ] Define aggregate candidates
- [ ] Define status lifecycle for OR
- [ ] Define stock movement lifecycle
- [ ] Define estimate versioning lifecycle
- [ ] Define QC closure rules
- [ ] Define invoice generation rules

### P0.3 Technical architecture
- [ ] Finalize backend architecture decision
- [ ] Finalize frontend architecture decision
- [ ] Define solution structure
- [ ] Define coding standards
- [ ] Define branching strategy
- [ ] Define environment strategy (dev/test/preprod/prod)
- [ ] Define API versioning approach
- [ ] Define logging and error strategy

---

## 5. Backend foundation tasks

### B1. Solution bootstrap
- [ ] Create ASP.NET Core solution structure
- [ ] Create projects: Api, Application, Domain, Infrastructure, Contracts, SharedKernel
- [ ] Add dependency graph guards
- [ ] Add architecture tests
- [ ] Add shared result/error abstractions
- [ ] Add base entity / audit abstractions

### B2. Persistence foundation
- [ ] Set up EF Core
- [ ] Define base DbContext strategy per module
- [ ] Configure migrations pipeline
- [ ] Add database seeding framework
- [ ] Add audit fields and soft-delete policy where needed
- [ ] Add optimistic concurrency support

### B3. Application foundation
- [ ] Add command/query pipeline
- [ ] Add validation pipeline behavior
- [ ] Add transaction pipeline behavior
- [ ] Add exception mapping strategy
- [ ] Add domain event dispatch strategy
- [ ] Add outbox-ready abstraction for future async reliability

### B4. Cross-cutting services
- [ ] File storage abstraction
- [ ] PDF generation abstraction
- [ ] Notification abstraction (email/SMS/WhatsApp)
- [ ] OCR abstraction
- [ ] Time provider abstraction
- [ ] Current user abstraction

---

## 6. Frontend foundation tasks

### F1. React app bootstrap
- [ ] Create React + TypeScript app
- [ ] Set up feature-based folder structure
- [ ] Configure routing
- [ ] Configure server-state library
- [ ] Configure form library and validation
- [ ] Configure design system foundations

### F2. Application shell
- [ ] Authenticated layout
- [ ] Role-aware navigation
- [ ] Error boundary
- [ ] Toast/notification system
- [ ] Global loading states
- [ ] Common table / detail / form components

### F3. Shared UX building blocks
- [ ] Media upload component
- [ ] Signature component
- [ ] Status badge component
- [ ] Timeline component
- [ ] Dashboard cards
- [ ] Reusable filters/search patterns

---

## 7. Security & RBAC tasks

### S1. Authentication
- [ ] Login endpoint
- [ ] Token issuance
- [ ] Refresh token support
- [ ] Logout endpoint
- [ ] Session expiration strategy

### S2. Authorization
- [ ] Roles seed data
- [ ] Permission catalog
- [ ] Policy-based authorization
- [ ] UI access guards by role
- [ ] Audit log on sensitive actions

### S3. Security hardening
- [ ] Input validation
- [ ] Secure file upload validation
- [ ] API rate protections for auth endpoints
- [ ] Error message sanitization
- [ ] Secure document access links

---

## 8. Module 1 — Reception du véhicule

### Backend
- [ ] Customer entity and repository contracts
- [ ] Vehicle entity and repository contracts
- [ ] RepairOrder entity initial version
- [ ] Create customer use case
- [ ] Create vehicle use case
- [ ] Create repair order use case
- [ ] OR numbering strategy
- [ ] Intake photo upload use case
- [ ] OR type selection rule
- [ ] Internal company vehicle handling rule

### Frontend
- [ ] Customer search/create screen
- [ ] Vehicle search/create screen
- [ ] Intake form screen
- [ ] OR type selector UI
- [ ] Intake photo capture/upload UI
- [ ] Optional send OR to customer action
- [ ] Internal account toggle / workflow

### Business rules / validation
- [ ] Validate required intake fields
- [ ] Prevent duplicate vehicle records by strong identifiers
- [ ] Ensure OR cannot start without valid customer + vehicle link

### Tests
- [ ] Unit tests for OR creation rules
- [ ] Integration tests for intake flow
- [ ] UI tests for reception flow

---

## 9. Module 2 — Diagnostic & Devis

### Backend
- [ ] Technician profile and skills model
- [ ] Technician assignment use case
- [ ] Diagnostic aggregate
- [ ] Add diagnostic notes/media use case
- [ ] Vehicle history query
- [ ] Estimate aggregate + version model
- [ ] Estimate generation use case
- [ ] Technical validation use case

### Frontend
- [ ] Technician assignment screen
- [ ] Diagnostic detail screen
- [ ] Notes/photos UI
- [ ] Vehicle history panel
- [ ] Estimate builder UI
- [ ] Internal validation screen

### Business rules
- [ ] Technician assignment suggestion rule
- [ ] Estimate version initialization rule
- [ ] Diagnostic completeness rule before estimate generation

### Tests
- [ ] Domain tests for assignment logic
- [ ] Estimate generation tests
- [ ] Integration flow from OR to estimate draft

---

## 10. Module 3 — Suivi devis & approbation

### Backend
- [ ] Estimate status machine
- [ ] Estimate version history model
- [ ] Send estimate notification use case
- [ ] Record client approval use case
- [ ] Record client refusal use case
- [ ] Auto-reminder background job
- [ ] Approval proof document handling

### Frontend
- [ ] Estimates inbox/list view
- [ ] Estimate detail view
- [ ] Version history UI
- [ ] Send/remind actions
- [ ] Approval status timeline

### Business rules
- [ ] Prevent repair flow before accepted estimate
- [ ] Preserve immutable history of prior estimate versions
- [ ] Support refused estimate closure trigger path

### Tests
- [ ] Estimate status transition tests
- [ ] Reminder scheduling tests
- [ ] Integration test for accepted flow
- [ ] Integration test for refused flow

---

## 11. Module 4 — Commande & Réception Pièces

### Backend
- [ ] Stock item model
- [ ] Warehouse movement model
- [ ] Supplier model
- [ ] Supplier order model
- [ ] Workshop part request use case
- [ ] Reserve stock use case
- [ ] Create supplier order use case
- [ ] Receive supplier order use case
- [ ] Issue part to OR use case
- [ ] Counter-sale order model and use case

### Frontend
- [ ] Magasinier dashboard
- [ ] Workshop requests view
- [ ] Stock lookup view
- [ ] Supplier orders view
- [ ] Reception screen
- [ ] Issue-to-OR screen
- [ ] Counter-sale screen

### Business rules
- [ ] No stock issue without traceable movement
- [ ] No negative stock unless explicit policy allows it
- [ ] Part issue must link to OR or counter-sale
- [ ] Delayed supplier lines should raise alert flag

### Tests
- [ ] Stock reservation tests
- [ ] Stock movement tests
- [ ] Supplier order reception tests
- [ ] Counter-sale flow tests

---

## 12. Module 5 — Suivi de Réparation

### Backend
- [ ] Repair execution aggregate
- [ ] Work log model
- [ ] Start repair use case
- [ ] Pause repair use case
- [ ] Resume repair use case
- [ ] Mark waiting-part use case
- [ ] Mark waiting-lead-decision use case
- [ ] Complete repair use case
- [ ] Delay detection logic
- [ ] External intervention flagging
- [ ] Road-test record model/use case

### Frontend
- [ ] Technician work queue
- [ ] Technician OR execution screen
- [ ] Start/pause/complete controls
- [ ] Pause reason UI
- [ ] Workshop lead live dashboard
- [ ] Priority / delay view
- [ ] External intervention / road test controls

### Business rules
- [ ] Repair cannot start before required estimate approval
- [ ] Waiting-part must stop active work timer
- [ ] OR cannot move to QC until repair marked complete

### Tests
- [ ] Work-status transition tests
- [ ] Delay alert tests
- [ ] Integration tests for repair tracking

---

## 13. Module 6 — Contrôle Qualité

### Backend
- [ ] QC checklist template model
- [ ] QC checklist instance model
- [ ] Add QC item evidence use case
- [ ] Add technical notes use case
- [ ] Lead validation use case
- [ ] Optional QC report generation use case
- [ ] OR closure trigger on QC validation

### Frontend
- [ ] Technician checklist UI
- [ ] Workshop lead QC validation screen
- [ ] QC photo upload UI
- [ ] QC notes UI
- [ ] QC report preview/download UI

### Business rules
- [ ] QC validation mandatory before invoice generation
- [ ] QC must store validation actor and timestamp
- [ ] Closed OR should be immutable except controlled billing actions

### Tests
- [ ] QC validation tests
- [ ] OR closure trigger tests
- [ ] Integration QC-to-billing tests

---

## 14. Module 7 — Facturation & Restitution

### Backend
- [ ] Invoice aggregate
- [ ] Invoice line generation from OR
- [ ] Estimate vs invoice comparison rule
- [ ] Payment record model
- [ ] Register payment use case
- [ ] Document sending use case
- [ ] Vehicle restitution use case

### Frontend
- [ ] Invoice detail screen
- [ ] Payment capture UI
- [ ] Payment status view
- [ ] Restitution confirmation screen
- [ ] Send invoice/checklist action

### Business rules
- [ ] No invoice generation before QC validation
- [ ] No vehicle restitution without invoice/payment state handled
- [ ] Delivery trace must store receiver identity

### Tests
- [ ] Invoice generation tests
- [ ] Payment capture tests
- [ ] Restitution flow tests

---

## 15. Notifications & documents

### Notifications
- [ ] Template system for email/SMS/WhatsApp
- [ ] Estimate-ready notification
- [ ] Estimate reminder notification
- [ ] Vehicle-ready notification
- [ ] Invoice sent notification

### Documents
- [ ] OR PDF summary
- [ ] Estimate PDF
- [ ] QC checklist PDF
- [ ] Quality report PDF
- [ ] Invoice PDF
- [ ] Delivery confirmation PDF or audit record

### Background processing
- [ ] Background job host
- [ ] Retry strategy
- [ ] Failure logging
- [ ] Delivery status persistence

---

## 16. Reporting & dashboard MVP

### Operational dashboards
- [ ] ORs in progress
- [ ] Delayed ORs
- [ ] Awaiting parts
- [ ] Awaiting quality
- [ ] Ready for delivery

### Financial dashboards
- [ ] Invoiced amount
- [ ] Paid vs unpaid
- [ ] Parts consumed value
- [ ] Labor billed

### Stock dashboards
- [ ] Low stock alerts
- [ ] Supplier delays
- [ ] Stock value snapshot

### Technical dashboards
- [ ] Technician active workload
- [ ] Technician billed productivity placeholder
- [ ] Return-tech rate placeholder

---

## 17. Testing & quality engineering

### Automated tests
- [ ] Unit tests for all core domain rules
- [ ] Integration tests for all core workflows
- [ ] API contract tests
- [ ] Architecture tests
- [ ] Frontend component tests
- [ ] Frontend e2e tests for critical flows

### Quality gates
- [ ] CI build
- [ ] Test execution in CI
- [ ] Static analysis
- [ ] Formatting/linting
- [ ] Migration validation checks

---

## 18. DevOps & observability

### DevOps
- [ ] Dockerize backend
- [ ] Dockerize frontend
- [ ] Local compose setup
- [ ] CI pipeline
- [ ] CD pipeline for non-prod
- [ ] Secrets management strategy

### Observability
- [ ] Structured logging
- [ ] Correlation IDs
- [ ] Health checks
- [ ] Audit log storage
- [ ] Error monitoring integration

### Environment tasks
- [ ] Dev environment
- [ ] Test environment
- [ ] Preprod environment
- [ ] Production release checklist

---

## 19. Data migration / seed / configuration tasks

- [ ] Seed core roles and permissions
- [ ] Seed OR types
- [ ] Seed repair status catalog
- [ ] Seed QC checklist templates
- [ ] Seed notification templates
- [ ] Seed payment method catalog
- [ ] Seed stock movement types

---

## 20. MVP milestones

### Milestone 1 — Architecture ready
- [ ] Backend solution created
- [ ] Frontend shell created
- [ ] Auth/RBAC skeleton ready
- [ ] CI pipeline active

### Milestone 2 — Intake to estimate ready
- [ ] Module 1 complete
- [ ] Module 2 complete
- [ ] Module 3 complete
- [ ] Accepted and refused estimate flows testable

### Milestone 3 — Parts to repair ready
- [ ] Module 4 complete
- [ ] Module 5 complete
- [ ] Live workshop dashboard usable

### Milestone 4 — Close and bill ready
- [ ] Module 6 complete
- [ ] Module 7 complete
- [ ] Documents and notifications ready

### Milestone 5 — Production readiness
- [ ] Reporting essentials ready
- [ ] Test suite stable
- [ ] Security review done
- [ ] Pilot deployment ready

---

## 21. Recommended prioritization (Must / Should / Later)

### Must-have for MVP
- [ ] All modules 1 to 7 in core usable form
- [ ] RBAC
- [ ] Audit trail
- [ ] Notifications for estimate and vehicle ready
- [ ] PDF estimate and invoice
- [ ] Stock issue linked to OR
- [ ] QC before invoice

### Should-have if time allows
- [ ] OCR integration
- [ ] Signature pad
- [ ] Counter-sale flow polished
- [ ] Basic dashboards polished
- [ ] Reminder automation refined

### Later / post-MVP
- [ ] Full HR module
- [ ] Loyalty features
- [ ] Advanced supplier integration
- [ ] Advanced BI
- [ ] Customer portal/mobile app

---

## 22. Open tasks requiring business clarification

- [ ] Clarify en-compte billing rules
- [ ] Clarify diagnostic fee application
- [ ] Clarify partial approval of estimate
- [ ] Clarify mandatory signatures
- [ ] Clarify supplier order approval chain
- [ ] Clarify external intervention billing
- [ ] Clarify payment-before-delivery exact policies
- [ ] Clarify final KPI list for dashboard

---

## 23. Suggested first implementation order

### Phase 1
- [ ] Architecture setup
- [ ] Auth/RBAC
- [ ] Customer/vehicle/OR creation

### Phase 2
- [ ] Diagnostic
- [ ] Estimate generation/versioning
- [ ] Approval/refusal flow

### Phase 3
- [ ] Inventory basics
- [ ] Supplier orders
- [ ] Issue part to OR

### Phase 4
- [ ] Repair execution tracking
- [ ] QC validation

### Phase 5
- [ ] Invoice/payment/restitution
- [ ] Notifications/documents
- [ ] Dashboards

### Phase 6
- [ ] Stabilization
- [ ] Testing hardening
- [ ] Pilot rollout

---

## 24. Definition of done for MVP

A feature is done only if:
- business rule implemented
- API exposed
- UI usable
- authorization applied
- audit logging applied where needed
- automated tests added
- error states handled
- documentation updated

