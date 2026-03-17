# TYREX — MODULE 01 CONTEXT DÉTAILLÉ

## 1. Purpose of this file
This file is a focused implementation context for **Module 01 — Reception / Vehicle Intake** of the TYREX automotive workshop platform.

It is intended for an AI coding agent such as GitHub Copilot / Copilot Chat / coding IDE agents.

This file must be used together with:
- `PROJECT_CONTEXT.md`
- `TASKS.md`

But for this implementation batch, this document is the **module-specific source of truth**.

The goal is to keep the AI focused on **one module at a time**, with precise business meaning, boundaries, workflows, data requirements, UI expectations, and implementation expectations.

---

## 2. Product context
TYREX is a digital platform for managing an automotive workshop end-to-end:
- reception
- repair order creation
- diagnostic
- quotation
- quotation approval / refusal
- parts ordering / stock
- repair tracking
- quality control
- billing
- vehicle delivery
- notifications and reporting

The platform is intended to replace paper-based and loosely coordinated workshop operations with a structured digital workflow.

The MVP is implemented as:
- **Backend**: ASP.NET Core Web API
- **Architecture**: Clean Architecture + DDD + modular monolith
- **Frontend**: React + TypeScript
- **Database**: PostgreSQL

---

## 3. Position of Module 01 in the global workflow
Module 01 is the **entry point** of the operational workflow.

Global flow:
1. Reception / vehicle intake
2. Repair order creation
3. Diagnostic
4. Quote creation
5. Quote approval/refusal
6. Parts / stock
7. Repair execution
8. Quality control
9. Billing
10. Delivery

Module 01 must produce the initial operational record that the rest of the platform will use.

If Module 01 is weak, all later modules become unreliable.

---

## 4. Business purpose of Module 01
Module 01 exists to:
- register the client and vehicle at arrival
- identify the visit reason
- capture the initial state of the vehicle
- classify the visit under the correct repair order type
- create the initial repair order record
- ensure traceability from the very first step
- support a mobile/tablet-based reception workflow
- optionally send the initial intake / OR summary to the customer

This module is not just a form. It establishes the legal, operational, and data foundation for the rest of the process.

---

## 5. Primary actors
### 5.1 Main actor
- **Receptionist / Service Advisor / Reception Agent**

### 5.2 Secondary actors
- **Customer**
- **Internal company vehicle requester**
- **Workshop manager / admin**

### 5.3 Downstream actors depending on Module 01 output
- Technician
- Team leader / workshop supervisor
- Parts / stock manager
- Billing / cashier

---

## 6. Core outcomes of Module 01
At the end of Module 01, the system must be able to produce the following:
- a valid **Repair Order (OR)** record
- linked customer record
- linked vehicle record
- selected OR type
- initial complaint / customer request
- vehicle initial condition / state report
- intake photos
- intake timestamps
- reception agent identity
- optional customer signature
- optional notification sent to customer

---

## 7. Business concepts involved
### 7.1 Customer
A person or organization that owns or operates the vehicle.

Possible categories:
- individual customer
- account customer / company customer
- internal company vehicle

### 7.2 Vehicle
A uniquely identifiable vehicle entering the workshop.

Typical attributes:
- plate number
- VIN / chassis number when available
- brand
- model
- fuel type if relevant
- mileage
- year if relevant
- color if useful

### 7.3 Repair Order (OR)
The main operational dossier of the workshop visit.
It is created at reception and then used by all downstream modules.

### 7.4 OR Type
The business classification of the reception case.
Expected types:
- `Sinister` / insurance / accident-related
- `General`
- `FastService`
- `TechnicalReturn`

### 7.5 Intake / Check-in
The physical and digital registration of the vehicle into workshop operations.

### 7.6 Initial state report
A record of the visible vehicle condition at the moment of reception.
Includes notes and photos.

### 7.7 OCR-assisted intake
Optional productivity feature where a document or plate can be scanned and partially parsed automatically.

---

## 8. Scope of Module 01
### 8.1 In scope
- customer search / create
- vehicle search / create
- OR creation
- OR type selection
- intake notes
- complaint capture
- intake photos
- mobile/tablet intake workflow
- optional OCR-based assisted data entry
- optional customer signature capture
- optional send of intake summary / OR to customer
- support internal company vehicle intake classification

### 8.2 Out of scope for this module
- technical diagnosis decision logic
- quote pricing logic
- stock movements
- repair execution tracking
- quality validation
- billing computation

Those belong to later modules.

---

## 9. Relations to other modules
### 9.1 Relation with Module 02 — Diagnostic & Quote
Module 02 consumes the OR created by Module 01.
Without a valid OR, diagnostic must not start.

Required output to Module 02:
- OR id / number
- customer identity
- vehicle identity
- OR type
- complaint / visit reason
- intake notes
- intake photos
- initial mileage

### 9.2 Relation with Module 03 — Quote approval
The quote generated later depends on the intake case classification and customer category defined in Module 01.

### 9.3 Relation with Module 04 — Parts / stock
Vehicle identity and OR identity created here will later be used to attach part consumption correctly.

### 9.4 Relation with Module 05 — Repair execution
Repair execution uses the OR created here as the operational anchor.

### 9.5 Relation with Module 06 — Quality
Initial intake condition can be compared with post-repair state when needed.

### 9.6 Relation with Module 07 — Billing / Delivery
Customer category and vehicle ownership information created here influences downstream billing and delivery logic.

---

## 10. Functional scenarios to support
### Scenario A — Standard individual customer
A customer arrives with a car for a normal service issue.

Expected flow:
1. Reception agent searches for the customer.
2. If not found, creates the customer.
3. Searches or creates the vehicle.
4. Records visit reason.
5. Selects OR type = `General`.
6. Captures intake photos.
7. Records visible initial condition.
8. Optionally captures signature.
9. Creates OR.
10. Optionally sends intake/OR summary to customer.

### Scenario B — Fast service reception
Customer arrives for a quick intervention (example: oil change / simple brake / tire service).

Expected flow:
1. Search/create customer.
2. Search/create vehicle.
3. Select OR type = `FastService`.
4. Enter quick complaint / request.
5. Minimal but valid intake.
6. Create OR quickly.
7. Make OR available to downstream fast-service flow.

### Scenario C — Insurance / accident case
Vehicle arrives after accident or insurance-related claim.

Expected flow:
1. Search/create customer.
2. Search/create vehicle.
3. Select OR type = `Sinister`.
4. Capture detailed visible damage photos.
5. Add intake notes.
6. Create OR.

### Scenario D — Technical return
Vehicle returns after a prior intervention.

Expected flow:
1. Search existing customer and vehicle.
2. Link or detect previous history.
3. Select OR type = `TechnicalReturn`.
4. Capture current complaint and photos.
5. Create OR.

### Scenario E — Internal company vehicle
A company/internal vehicle is received.

Expected flow:
1. Customer category or account category is internal/company.
2. Vehicle is registered under internal/company account.
3. OR is created with appropriate classification.
4. Later reporting can distinguish internal operations from external customer operations.

---

## 11. Detailed functional requirements
### 11.1 Customer search and creation
The UI must allow the receptionist to:
- search by phone number
- search by full/partial name
- search by company name when relevant
- create a new customer if none exists

Minimal customer fields for MVP:
- first name
- last name
- phone number
- email (optional but useful)
- customer category

### 11.2 Vehicle search and creation
The UI must allow the receptionist to:
- search vehicle by plate number
- search by VIN if available
- create a new vehicle

Minimal vehicle fields for MVP:
- plate number
- brand
- model
- VIN / chassis number (optional but recommended)
- mileage at intake
- owner/customer link

### 11.3 OR type selection
The receptionist must choose one of the supported OR types.
This choice affects downstream flow and reporting.

Required OR types:
- General
- Sinister
- FastService
- TechnicalReturn

### 11.4 Complaint / visit reason capture
The receptionist must be able to record the customer’s reason for visit.
This is not the technical diagnosis; it is the initial customer complaint/request.

Example values:
- brake noise
- oil change request
- AC not cooling
- vehicle returned after previous repair

### 11.5 Intake state capture
The system must allow recording:
- free-text visible condition notes
- optional structured notes if the team decides later
- photos of the vehicle at intake

Typical photos:
- front
- rear
- left side
- right side
- additional damage close-ups

### 11.6 OCR-assisted entry
For MVP this can be implemented in one of two ways:
- real OCR integration if feasible now
- placeholder-ready architecture with a clean extension point

The AI must not fake OCR. If real OCR is not implemented in the first batch, it must design the system so OCR can be added later without major refactor.

### 11.7 Signature capture
If implemented in MVP, the customer can sign at intake.
If not implemented fully, the architecture and UI extension point should be prepared clearly.

### 11.8 OR creation
Once required data is present, the system must create an OR.
Expected result:
- unique OR number
- OR status initialized correctly
- customer and vehicle linked
- type assigned
- complaint saved
- intake state saved
- photos linked
- audit trail created

### 11.9 Notification / send to customer
If included in current batch, the system can send an intake summary / OR summary through one of the supported channels.
For MVP, even if actual WhatsApp/SMS integration is deferred, the architecture must separate:
- document generation
- notification request creation
- channel adapters

---

## 12. Non-functional expectations for Module 01
- must be easy and fast to use by front-office staff
- must support workshop operational speed
- must be tablet-friendly / responsive
- must keep traceability and auditability
- must not allow inconsistent OR creation
- must be secure and role-restricted
- must be testable
- must be extensible for OCR/signature/notifications

---

## 13. UI / UX expectations
### 13.1 Main screens
#### Screen A — Customer search/create
- search input
- result list
- create new customer form

#### Screen B — Vehicle search/create
- plate search
- vehicle result list
- create vehicle form

#### Screen C — Intake / OR form
- selected customer
- selected vehicle
- OR type selector
- complaint field
- mileage field
- intake notes
- photo upload
- optional signature area
- create OR action

#### Screen D — OR result screen
- created OR number
- summary information
- send to customer action
- proceed to next operational step

### 13.2 UX principles
- minimal clicks
- mobile/tablet compatible
- receptionist-friendly wording
- clear validation messages
- status feedback after save

---

## 14. Backend implementation expectations
### 14.1 Architectural boundaries
Module 01 should be implemented as its own module within the modular monolith.

Suggested module name:
- `Reception`

### 14.2 Expected internal layers
Inside the module, separate at least:
- Domain
- Application
- Infrastructure
- API

### 14.3 Domain candidates
Potential domain objects:
- `RepairOrder`
- `Customer`
- `Vehicle`
- `VehicleIntakePhoto`
- `VehicleIntakeReport`
- `RepairOrderType` (enum/value object)

### 14.4 Aggregate thinking
Suggested aggregate root for this batch:
- `RepairOrder`

Potential reasoning:
- the OR is the operational anchor
- intake photos and intake report can belong to OR lifecycle
- customer and vehicle may be separate entities referenced by OR

The AI may refine this if it has a stronger DDD justification, but it must remain practical for MVP.

### 14.5 Application use cases
Expected application commands / use cases may include:
- CreateCustomer
- CreateVehicle
- CreateRepairOrder
- AddRepairOrderIntakePhotos
- SendRepairOrderSummary
- SearchCustomers
- SearchVehicles
- GetRepairOrderById
- ListRepairOrders

### 14.6 API expectations
Potential endpoints:
- `GET /api/customers/search`
- `POST /api/customers`
- `GET /api/vehicles/search`
- `POST /api/vehicles`
- `POST /api/repair-orders`
- `GET /api/repair-orders/{id}`
- `GET /api/repair-orders`
- `POST /api/repair-orders/{id}/photos`
- `POST /api/repair-orders/{id}/send-summary`

Exact routes can differ, but they must stay consistent and business-oriented.

---

## 15. Data model expectations
Minimum tables/entities likely needed in MVP:
- Customers
- Vehicles
- RepairOrders
- RepairOrderIntakePhotos
- RepairOrderEvents / Audit trail

Suggested RepairOrders fields:
- Id
- Number
- CustomerId
- VehicleId
- Type
- Status
- Complaint
- IntakeMileage
- IntakeNotes
- CreatedByUserId
- CreatedAtUtc
- UpdatedAtUtc

Suggested Vehicles fields:
- Id
- CustomerId or Owner reference
- PlateNumber
- Vin
- Brand
- Model
- Year (optional)
- CreatedAtUtc

Suggested Customers fields:
- Id
- FirstName
- LastName
- CompanyName (optional)
- Phone
- Email
- Category
- CreatedAtUtc

Suggested IntakePhotos fields:
- Id
- RepairOrderId
- FilePath or blob reference
- PhotoType (optional)
- UploadedAtUtc

---

## 16. Business rules
### Required rules
1. A Repair Order cannot be created without a customer.
2. A Repair Order cannot be created without a vehicle.
3. A Repair Order must have a valid OR type.
4. A Repair Order must have at least a minimal complaint / reason for visit.
5. Repair Order numbering must be unique.
6. Vehicle plate numbers should be normalized.
7. Customer phone should be normalized.
8. Only authorized front-office roles may create ORs.
9. The created OR must be auditable.

### Recommended rules
10. Mileage should be captured at intake when available.
11. Duplicate vehicle creation should be reduced through search + validation.
12. Duplicate customer creation should be reduced through search + validation.
13. Intake photo upload should tolerate multiple images.
14. Internal/company vehicle intake must remain identifiable.

---

## 17. Validation rules
Examples:
- customer full name required for individual customers
- phone required in MVP unless explicitly decided otherwise
- plate number required for vehicle unless a special business exception exists
- OR type required
- complaint required
- mileage non-negative

These should be implemented with clear API and frontend validation.

---

## 18. Security / authorization expectations
Expected role-based constraints:
- Receptionist / Service Advisor can create customers, vehicles, and ORs
- Admin can view and manage all
- Non-front-office roles should not have unrestricted OR creation by default

Use JWT-based auth and permission/role-based authorization consistent with the project foundation.

---

## 19. Document / notification expectations
For MVP, the following are acceptable implementation levels:

### Level A — Strong MVP
- generate intake / OR summary PDF
- create notification request
- send via at least one working channel (email is acceptable as first channel)

### Level B — Acceptable staged MVP
- generate OR summary
- expose a “send summary” action
- persist notification request and channel data
- leave external adapter implementation behind a clear interface if not finished yet

But the AI must not pretend full WhatsApp/SMS delivery exists if it does not.

---

## 20. Testing expectations
The AI must implement tests for Module 01.

### Backend tests
- create customer success
- create vehicle success
- create OR success
- create OR fails without required data
- only allowed roles can create OR
- OR number is generated correctly / uniquely

### Integration tests
- create customer + vehicle + OR end-to-end through API
- upload intake photos

### Frontend tests
- intake form validation
- OR creation success flow
- customer/vehicle search interaction where practical

---

## 21. Acceptance criteria for Module 01
Module 01 is considered functionally usable when all of the following are true:
1. A receptionist can search or create a customer.
2. A receptionist can search or create a vehicle.
3. A receptionist can choose an OR type.
4. A receptionist can enter complaint + intake data.
5. A receptionist can upload intake photos.
6. A receptionist can create a valid repair order.
7. The repair order is saved in DB with correct links.
8. The repair order can be opened later by downstream modules.
9. The UI is usable on desktop and tablet.
10. Basic tests pass.

---

## 22. Explicit implementation guidance for Copilot / AI agent
You are not allowed to stop at setup.
You must implement the real Module 01.

### Your job for this batch
1. Audit the current repo state.
2. Identify what already exists for Module 01.
3. Identify what is only scaffold.
4. Complete the missing domain/application/infrastructure/API/frontend parts.
5. Add migrations.
6. Add tests.
7. Ensure Module 01 works end-to-end.

### Important constraints
- do not rebuild the entire project from scratch
- continue from the current repository state
- refactor weak/basic scaffolding where needed
- keep code production-minded
- keep architecture clean and practical
- keep module boundaries explicit

### Expected delivery from the AI
The AI should return:
- repository audit for Module 01
- implementation plan for Module 01
- generated or updated files
- commands to run migrations/tests
- verification checklist

---

## 23. Suggested implementation sequence for Module 01
1. Audit existing repo
2. Finalize domain model for customer/vehicle/repair order
3. Implement persistence mappings
4. Implement application commands/queries
5. Implement API endpoints
6. Implement frontend screens/forms
7. Implement file upload support for intake photos
8. Implement basic send-summary/document hook
9. Add tests
10. Validate end-to-end workflow

---

## 24. What not to do
- do not jump to later modules before Module 01 is operational
- do not create fake placeholder code and call it done
- do not create a huge generic ERP abstraction layer for no reason
- do not leave the OR creation flow incomplete
- do not skip validation or tests
- do not hide unfinished functionality behind misleading wording

---

## 25. Completion definition
Module 01 is complete for this focused batch only when:
- backend endpoints work
- DB persists all needed data
- frontend allows the full reception flow
- OR can actually be created end-to-end
- tests are present and passing
- downstream modules can consume the created OR

