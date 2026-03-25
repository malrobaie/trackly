# Trackly

Trackly is a package tracking dashboard built with Angular and ASP.NET Core.

## Overview
Trackly gives a user one place to save and view package tracking updates instead of checking multiple carrier sites manually. The current version supports:
- adding a tracked package with a selected carrier
- viewing tracked packages in a dashboard
- opening a package details page with tracking history
- refreshing a package status
- deleting a tracked package

Right now, Trackly is a working local demo. It is structured so it can grow into a real user-based product with live carrier integrations and login-based ownership.

## Current State
- Full local demo flow is working
- Frontend and backend communicate successfully
- In-memory storage is used for tracked packages
- USPS is wired for live API support with mock fallback
- UPS, FedEx, DHL, and OnTrac are mock-backed
- User login is not implemented yet

## Technologies Used

### Frontend
- Angular
- TypeScript
- Angular Router
- Angular HttpClient
- Reactive Forms
- CSS

### Backend
- ASP.NET Core Web API
- C#
- Swagger / OpenAPI
- Dependency Injection
- HttpClient

### Integration
- USPS OAuth2 and Tracking API support is wired into the backend
- Provider abstraction is used so each carrier can have its own implementation

## Infrastructure And Architecture

### High-Level Structure
```text
trackly/
  backend/
  frontend/
  TRACKLY_BUILD_CHECKLIST.md
  README.md
```

### Backend Responsibilities
- expose REST endpoints
- validate requests
- manage tracked package records
- normalize carrier-specific responses
- support carrier providers through an orchestrator pattern

### Frontend Responsibilities
- display dashboard, add form, and details view
- handle validation and user actions
- call backend endpoints
- render tracking summaries and timelines

### Current Data And Service Flow
1. User submits a carrier and tracking number from Angular
2. Angular sends the request to the ASP.NET API
3. Backend creates or refreshes a tracked package
4. Backend asks the appropriate carrier provider for tracking data
5. Provider returns normalized tracking data
6. Backend returns a frontend-friendly response

### Carrier Provider State
- USPS: live integration path is implemented, mock fallback enabled by default
- UPS: mock provider
- FedEx: mock provider
- DHL: mock provider
- OnTrac: mock provider

## Run The App

### Backend
From the repo root:

```powershell
dotnet run --project .\backend
```

Swagger:
```text
http://localhost:5157/swagger
```

### Frontend
From the repo root:

```powershell
cd .\frontend
npm start
```

Frontend:
```text
http://localhost:4200
```

## Build Checks

### Backend
```powershell
dotnet build .\backend
```

### Frontend
```powershell
cd .\frontend
npm run build
```

## First-Time Setup

### Backend
Only if needed:

```powershell
dotnet restore .\backend
```

### Frontend
Only if needed:

```powershell
cd .\frontend
npm install
```

## USPS API Setup
To enable live USPS requests, set these before starting the backend:

```powershell
$env:UspsApi__ClientId="your-usps-consumer-key"
$env:UspsApi__ClientSecret="your-usps-consumer-secret"
```

Optional:

```powershell
$env:UspsApi__UseTestEnvironment="true"
```

Current behavior:
- if USPS credentials are present, Trackly will try the live USPS API
- if USPS credentials are missing or USPS is unavailable, Trackly falls back to mock USPS data

## What Is Still Needed Before Production

### Product Basics
- real user login
- user-owned tracked packages
- persistent database storage
- removal of anonymous global access

### Carrier Integration
- validate USPS live mapping with real credentials and real tracking numbers
- decide which other carriers should move from mock to live
- handle rate limiting, retries, and external API outages more formally

### Security
- authentication and authorization
- secure secret management
- environment-based configuration strategy
- production-safe logging and error handling

### Data And Persistence
- replace in-memory storage with a real database
- add migrations and durable storage strategy
- define user/package ownership schema

### Reliability
- automated tests
- backend integration tests
- frontend component and flow tests
- health checks and deployment diagnostics

### Operations
- production deployment target
- CI/CD pipeline
- monitoring and alerting
- environment separation for local, test, and production

## Immediate Next Steps
- finish live USPS verification once credentials are available
- choose the auth approach for user login
- decide whether persistence should land before or during auth
- implement login and user-owned tracking

## Notes
- Do not run `npm audit fix --force`
- The current npm audit warning is from Angular tooling dependencies
- The detailed build tracker lives in `TRACKLY_BUILD_CHECKLIST.md`
