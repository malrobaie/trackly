# Trackly

Trackly is a package tracking dashboard built with Angular and ASP.NET Core.

## Current Status
- Phase 0 complete
- Phase 1 complete
- Backend scaffold is working
- Frontend scaffold is working
- Auth is planned for a later phase so users can see only their own tracked items

## What We Have Done
- Created `backend/` and `frontend/`
- Set up the ASP.NET Core API
- Enabled Swagger
- Added a health endpoint at `GET /api/health`
- Set up the Angular app with routing
- Added placeholder pages for dashboard, add tracking, and tracking details
- Added the project checklist

## Run The App

### Backend
From the repo root:

```powershell
dotnet run --project .\backend
```

### Frontend
From the repo root:

```powershell
cd .\frontend
npm start
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

## Notes
- Do not run `npm audit fix --force`
- The current audit warning is from Angular tooling dependencies
- For now, just use `npm start` and `npm run build`

## Next Step
Phase 2: define the models and API contracts.
