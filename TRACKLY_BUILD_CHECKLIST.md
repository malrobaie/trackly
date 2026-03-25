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

## Phase 8: Live USPS Integration
Goal: replace the current USPS mock path with a verified live USPS API flow and keep the existing UI contract stable.

### Step 1: USPS Developer Access
- [ ] Create or confirm USPS developer portal access
- [ ] Register the Trackly app in the USPS developer portal
- [ ] Confirm the app has access to the tracking product
- [ ] Obtain the USPS consumer key and consumer secret
- [ ] Confirm whether USPS also requires CRID and MID for your account setup

### Step 2: Local Configuration
- [x] Set `UspsApi__ClientId`
- [x] Set `UspsApi__ClientSecret`
- [x] Decide whether to use production or USPS test environment
- [x] Verify config is loaded without hardcoding secrets in source control

### Step 3: OAuth Token Flow
- [x] Request a USPS OAuth token from `https://apis.usps.com/oauth2/v3/token`
- [x] Cache the access token in the backend
- [x] Refresh the token before expiration
- [x] Return a clean `503` if token acquisition fails

### Step 4: USPS Tracking Request
- [x] Send a live request to the USPS tracking endpoint for a real tracking number
- [x] Log the raw USPS response shape during development
- [ ] Confirm the actual response fields used for status, timestamps, and location data

### Step 5: Response Mapping
- [x] Map the live USPS response into `CarrierTrackingSnapshotDto`
- [x] Normalize live USPS statuses into Trackly-friendly values
- [x] Normalize tracking events into the existing event DTO shape
- [x] Keep frontend contracts unchanged

### Step 6: Error Handling
- [x] Handle invalid tracking numbers
- [x] Handle USPS auth failures
- [x] Handle USPS rate limits or temporary outages
- [x] Return clean error messages without exposing secrets
- [x] Fall back to mock USPS data when live credentials are not ready

### Step 7: Verification
- [x] Create a tracked USPS package through the app
- [ ] Verify the dashboard shows live USPS status
- [ ] Verify the details page shows live USPS tracking events
- [ ] Verify refresh pulls updated live USPS data
- [x] Verify non-USPS carriers still work through the mock fallback
- [x] Verify USPS still works through mock fallback when credentials are missing

### Exit Criteria
- USPS requests are live when credentials are configured
- USPS falls back cleanly to mock data when credentials are unavailable
- USPS responses are normalized cleanly
- Existing frontend flow still works without contract changes

## Phase 9: User Login And Owned Tracking
Goal: add real user login so tracked packages belong to a user and only that user can view or manage them.

### Step 1: Auth Decision
- [ ] Choose auth provider approach
- [ ] Decide whether to use ASP.NET Identity, Clerk, Auth0, Firebase Auth, or another provider
- [ ] Decide whether Angular will use cookie auth or bearer token auth
- [ ] Confirm the minimal version 1 auth scope

### Step 2: Data Model Changes
- [ ] Add a user model or user identity integration
- [ ] Add `userId` ownership to tracked packages
- [ ] Replace in-memory-only assumptions where needed
- [ ] Decide whether to move to SQLite for persistence before auth lands

### Step 3: Backend Auth Plumbing
- [ ] Configure authentication middleware
- [ ] Configure authorization middleware
- [ ] Add a current-user accessor in the backend
- [ ] Make tracking service operations user-aware

### Step 4: Protect Tracking Endpoints
- [ ] Require authentication for tracking endpoints
- [ ] Filter `GET /api/tracking` by current user
- [ ] Restrict `GET /api/tracking/{id}` by ownership
- [ ] Restrict refresh by ownership
- [ ] Restrict delete by ownership
- [ ] Restrict create so new tracked items are saved to the current user

### Step 5: Frontend Auth Flow
- [ ] Add login page or redirect flow
- [ ] Add logout action
- [ ] Store auth state safely
- [ ] Protect tracking routes from anonymous access
- [ ] Show signed-in versus signed-out UI states

### Step 6: Frontend User-Aware Tracking Flow
- [ ] Prevent anonymous users from adding tracking items
- [ ] Load only the signed-in user's packages on dashboard
- [ ] Preserve details, refresh, and delete flow for owned packages only
- [ ] Show a clear message when the session expires

### Step 7: Verification
- [ ] User A can add and view their tracked packages
- [ ] User B cannot view User A packages
- [ ] Anonymous users cannot access tracking pages
- [ ] Login and logout work cleanly
- [ ] Refresh and delete remain restricted to the owner

### Exit Criteria
- Users must log in to view tracked items
- Tracked packages are owned by a user
- Anonymous global access is removed from the main tracking flow

## What Is Complete Now
- [x] Local Trackly v1 demo is complete
- [x] Backend CRUD works with normalized tracking data
- [x] Frontend flow works end to end
- [x] Provider abstraction is in place
- [x] USPS provider supports live integration and mock fallback
- [x] The app is usable today without waiting for USPS credentials

## What We Can Do Next
### Immediate Next Work
- [ ] Finish live USPS verification once developer credentials are available
- [ ] Confirm the real USPS payload mapping against live tracking responses
- [ ] Decide auth approach for login and owned tracking
- [ ] Decide whether to add SQLite before or during auth work

### After That
- [ ] Implement login flow
- [ ] Make tracked packages user-owned
- [ ] Restrict tracking endpoints by signed-in user
- [ ] Add persistent storage for users and packages

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
- [x] Invalid submissions are clear
- [x] Failed API calls do not break the UI
- [x] Main screens feel presentable

### Exit Criteria
- App is polished enough for a walkthrough demo

## Phase 7: Final Verification
Goal: confirm the project is stable before calling version 1 complete.

### Backend Checks
- [x] Swagger works for every endpoint
- [x] Route contracts match frontend expectations
- [x] Error responses are consistent
- [x] No dead placeholder code remains in the main flow

### Frontend Checks
- [x] Navigation works between all pages
- [x] Dashboard renders correctly with zero, one, and many items
- [x] Details page handles missing id or missing package
- [x] Form validation works for empty and invalid input

### End-To-End Scenarios
- [x] Add package
- [x] View dashboard
- [x] View details
- [x] Refresh details
- [x] Delete package
- [x] Retry after an API failure

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
- [x] Phase 6
- [x] Phase 7
- [ ] Phase 8
- [ ] Phase 9

### Current Critical Path
- [x] Backend scaffold
- [x] Frontend scaffold
- [x] Contracts
- [x] CRUD
- [x] UI wiring
- [x] Refresh
- [x] Carrier integration
- [x] Polish
- [x] Final verification
- [ ] Live USPS verification
- [ ] Auth and ownership

## Recommended Next Order
1. Finish Phase 8 with real USPS credentials
2. Confirm live USPS mapping with real tracking numbers
3. Decide auth provider and persistence path
4. Complete Phase 9 login and per-user tracking
