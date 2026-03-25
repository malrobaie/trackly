# Trackly Build Checklist

This document turns the original project outline into a build-ready execution plan for today. We will use it as the working checklist while we build each layer of the app.

## Build Objective
- Build a small full stack package tracking dashboard with Angular and ASP.NET Core
- Establish the foundation for per-user package tracking with authenticated access
- Support manual carrier selection
- Save tracked packages
- View package summaries and shipment history
- Refresh package status on demand
- Finish with a stable local demo

## Version 1 Guardrails
- Keep scope to 1 carrier first
- Add a second carrier only if the first integration is complete and stable
- No authentication in today's build
- No background jobs
- No auto-detection of carrier
- No deployment work until local app flow is complete

## Product Requirement To Preserve
- The real app should require user login so each user can view and manage only their own tracked items
- Authentication and user-linked data storage are required for the production-ready direction of Trackly
- Today's build will not implement auth, but the architecture should avoid blocking that addition later

## Definition Of Done For Today
- Backend runs locally with Swagger
- Frontend runs locally and can call backend successfully
- User can add a tracking number with a selected carrier
- User can view tracked items in a dashboard
- User can open a details view for one package
- User can refresh a package manually
- User can delete a tracked package
- At least one carrier path works with realistic mock data or live integration
- The codebase is organized enough to extend cleanly
- The codebase is organized so authentication and per-user ownership can be added without major rewrites

## Recommended Build Order
1. Create backend
2. Create frontend
3. Define contracts and domain models
4. Implement in-memory CRUD flow
5. Connect Angular to backend
6. Build details page
7. Add refresh flow
8. Integrate first carrier
9. Improve validation, states, and styling
10. Test main flows end to end

## Phase 0: Working Decisions
Goal: lock the decisions that affect all later layers.

### Checkpoints
- [x] Confirm solution structure: `backend/` and `frontend/`
- [x] Confirm backend uses ASP.NET Core Web API
- [x] Confirm frontend uses Angular with routing and reactive forms
- [x] Confirm storage starts in memory
- [x] Confirm first carrier target
- [x] Confirm whether carrier integration starts with mock data or live API

### Output
- One agreed implementation path with no ambiguous stack decisions

### Decisions Locked For This Build
- [x] Repository structure will be `backend/` and `frontend/`
- [x] Backend will use ASP.NET Core Web API with Swagger enabled from the start
- [x] Frontend will use Angular with routing and reactive forms
- [x] Storage will start as in-memory only for version 1
- [x] First carrier target will be USPS
- [x] Carrier work will start with mock data first
- [x] We will still design the backend with a provider abstraction so live integration can replace mock logic later
- [x] We will not add SQLite until the main app flow is complete
- [x] We will leave authentication out of today's implementation, but we will treat per-user tracking as a required future layer
- [x] We will not initialize GitHub-related project setup until Phase 1 scaffold is in place

### Why This Path
- USPS is a reasonable first carrier for the domain, even if live API access is deferred
- Mock-first removes external API risk while we build the full app flow today
- In-memory storage keeps backend setup fast and reversible
- Provider abstraction lets us upgrade to live carrier integration without rewriting the frontend contract
- Delaying authentication keeps today's scope realistic, but calling it out now prevents us from designing the app as permanently anonymous

## Future Phase: Authentication And User Ownership
Goal: define the layer that turns Trackly from a local demo into a real user-based app.

### Required Capabilities
- [ ] Add login and logout flow
- [ ] Add user identity model
- [ ] Link tracked packages to a user id
- [ ] Restrict `GET /api/tracking` to the signed-in user's packages only
- [ ] Restrict `GET /api/tracking/{id}` to owned packages only
- [ ] Restrict refresh and delete actions to owned packages only
- [ ] Add frontend auth state and route protection
- [ ] Add persistent storage for users and tracked packages

### Architecture Notes
- Controllers and services should avoid assuming global anonymous access forever
- Tracking records should be designed so a `userId` can be added cleanly
- Frontend service design should allow authenticated API calls later

## Phase 1: Repository Setup
Goal: create the skeleton for both apps and verify they boot.

### Backend Tasks
- [x] Create ASP.NET Core Web API project
- [x] Enable Swagger/OpenAPI
- [x] Add folders:
  - [x] `Controllers`
  - [x] `Services`
  - [x] `Interfaces`
  - [x] `Models`
  - [x] `DTOs`
  - [x] `Enums` or `Constants`
- [x] Configure CORS for Angular local dev
- [x] Configure dependency injection
- [x] Register `HttpClient`

### Frontend Tasks
- [x] Create Angular app
- [x] Enable routing
- [x] Set up folders:
  - [x] `src/app/core`
  - [x] `src/app/shared`
  - [x] `src/app/features/dashboard`
  - [x] `src/app/features/add-tracking`
  - [x] `src/app/features/tracking-details`
  - [x] `src/app/models`
  - [x] `src/app/services`
- [x] Add app shell layout
- [x] Add placeholder routes

### Verification
- [x] Backend starts successfully
- [x] Swagger loads
- [x] Frontend starts successfully
- [x] Frontend route navigation works

### Exit Criteria
- Both applications boot locally without errors

## Phase 2: Domain Contracts
Goal: define the data model before implementing behavior.

### Backend Data Design
- [x] Create `Carrier` enum or approved constant list
- [x] Create `TrackedPackage` model
- [x] Create `TrackingEvent` model
- [x] Create `CreateTrackingRequestDto`
- [x] Create `TrackedPackageSummaryDto`
- [x] Create `TrackedPackageDetailsDto`
- [x] Create a normalized carrier response model if separate from persistence model

### Frontend Data Design
- [x] Create `Carrier` type or enum
- [x] Create `TrackedPackageSummary` interface
- [x] Create `TrackedPackageDetails` interface
- [x] Create `TrackingEvent` interface
- [x] Create `CreateTrackingRequest` interface

### Suggested Minimum Fields
#### TrackedPackage
- [x] `id`
- [x] `carrier`
- [x] `trackingNumber`
- [x] `status`
- [x] `estimatedDelivery`
- [x] `lastUpdated`
- [x] `events`

#### TrackingEvent
- [x] `timestamp`
- [x] `location`
- [x] `description`
- [x] `statusCode`

### Verification
- [x] Swagger models look correct
- [x] Frontend service models match backend payloads
- [x] No contract ambiguity remains

### Exit Criteria
- All core request and response shapes are locked

## Phase 3: Backend CRUD With Mock Tracking Data
Goal: complete the backend flow before any real carrier dependency.

### API Endpoints
- [x] `POST /api/tracking`
- [x] `GET /api/tracking`
- [x] `GET /api/tracking/{id}`
- [x] `POST /api/tracking/{id}/refresh`
- [x] `DELETE /api/tracking/{id}`

### Backend Service Tasks
- [x] Create in-memory repository or store
- [x] Create tracking service abstraction
- [x] Implement create flow
- [x] Implement get-all flow
- [x] Implement get-by-id flow
- [x] Implement delete flow
- [x] Implement refresh flow using mock status changes or regenerated sample data
- [x] Add validation for carrier and tracking number
- [x] Return consistent error responses

### Mock Data Rules
- [x] Generate believable status text
- [x] Generate event history in descending or clearly defined order
- [x] Keep normalized shape identical to future live integration

### Verification
- [x] All endpoints callable in Swagger
- [x] Create returns a saved tracked item
- [x] Get-all returns collection
- [x] Get-by-id returns full details
- [x] Refresh updates `lastUpdated`
- [x] Delete removes the item

### Exit Criteria
- Backend is fully usable with mock data only

## Phase 4: Frontend Core Flow
Goal: make the app usable end to end against the mock backend.

### App Routing
- [x] `/` dashboard route
- [x] `/add` add tracking route
- [x] `/tracking/:id` details route

### Services
- [x] Create Angular API service for tracking endpoints
- [x] Add environment config for backend base URL
- [x] Add request and response typing
- [x] Add basic error handling

### Dashboard Page
- [x] Load tracked packages on init
- [x] Show package cards or rows
- [x] Show carrier, tracking number, status, and last updated
- [x] Add delete action
- [x] Add link to details
- [x] Add empty state

### Add Tracking Page
- [x] Build reactive form
- [x] Add carrier dropdown
- [x] Add tracking number input
- [x] Add required validation
- [x] Submit to backend
- [x] Redirect to dashboard or details on success
- [x] Show submission errors

### Details Page
- [x] Load tracked item by route id
- [x] Show package summary
- [x] Show event timeline
- [x] Add refresh button
- [x] Add loading and error states

### Verification
- [x] User can add a package from the UI
- [x] New package appears in dashboard
- [x] User can open details page
- [x] User can delete a package
- [x] User can refresh a package from details

### Exit Criteria
- Full CRUD plus refresh works from the browser

## Phase 5: Carrier Integration Layer
Goal: swap backend mock generation for a real provider design.

### Architecture Checkpoints
- [x] Create `ITrackingProvider`
- [x] Create provider selection strategy by carrier
- [x] Create orchestrator service for provider calls
- [x] Keep controller contracts unchanged
- [x] Keep frontend unchanged except for better loading/error handling

### First Carrier Implementation
- [x] Select first carrier provider
- [x] Add provider-specific request building
- [x] Add provider-specific response mapping
- [x] Normalize response to Trackly model
- [x] Handle carrier failure and not-found cases
- [x] Decide fallback when live API is unavailable

### Verification
- [x] First carrier returns normalized data
- [x] Existing dashboard/details pages need no contract changes
- [x] Refresh still works

### Exit Criteria
- One carrier path works through the provider abstraction

## Phase 6: Validation, UX, and Error Handling
Goal: make the app clear and reliable for demo use.

### Backend
- [x] Improve validation messages
- [x] Return proper `404` for missing item
- [x] Return proper `400` for invalid request
- [x] Handle provider failures cleanly

### Frontend
- [x] Disable submit during request
- [x] Show form validation messages
- [x] Show loading indicators
- [x] Show empty states
- [x] Show error messages for failed API calls
- [x] Add status badges
- [x] Improve spacing and visual hierarchy
- [x] Make timeline readable

### Verification
- [ ] Invalid submissions are clear
- [ ] Failed API calls do not break the UI
- [ ] Main screens feel presentable

### Exit Criteria
- App is polished enough for a walkthrough demo

## Phase 7: Final Verification
Goal: confirm the project is stable before calling version 1 complete.

### Backend Checks
- [ ] Swagger works for every endpoint
- [ ] Route contracts match frontend expectations
- [ ] Error responses are consistent
- [ ] No dead placeholder code remains in the main flow

### Frontend Checks
- [ ] Navigation works between all pages
- [ ] Dashboard renders correctly with zero, one, and many items
- [ ] Details page handles missing id or missing package
- [ ] Form validation works for empty and invalid input

### End-To-End Scenarios
- [ ] Add package
- [ ] View dashboard
- [ ] View details
- [ ] Refresh details
- [ ] Delete package
- [ ] Retry after an API failure

### Nice-To-Have If Time Remains
- [ ] Add SQLite persistence
- [ ] Add second carrier
- [ ] Add reusable UI components
- [ ] Add unit tests
- [ ] Add seed/demo sample data

### Exit Criteria
- Trackly version 1 is demo ready locally

## Implementation Notes
- Build contracts first, then features
- Keep backend responses normalized so the frontend never needs carrier-specific logic
- Finish one carrier end to end before expanding
- Prefer complete simple behavior over partial advanced behavior
- Avoid refactoring too early until the first usable flow exists

## Session Tracker
Use this section during the build to mark current focus.

### Current Phase
- [x] Phase 0
- [x] Phase 1
- [x] Phase 2
- [x] Phase 3
- [x] Phase 4
- [x] Phase 5
- [ ] Phase 6
- [ ] Phase 7

### Current Critical Path
- [x] Backend scaffold
- [x] Frontend scaffold
- [x] Contracts
- [x] CRUD
- [x] UI wiring
- [x] Refresh
- [x] Carrier integration
- [ ] Polish
- [ ] Final verification
