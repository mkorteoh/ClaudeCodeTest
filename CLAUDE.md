# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Git

**Commit to git after every change.** After completing any modification — however small — stage the relevant files and create a commit with a detailed summarization of changes. Then push to GitHub.

## Projects

This repository contains two projects:

### InsuranceDMS (`InsuranceDMS/`)

A full-stack insurance distribution management system.

**Backend** — `InsuranceDMS/backend/`
- ASP.NET Core / .NET 6, Clean Architecture (Domain / Application / Infrastructure / API)
- Entity Framework Core 6 with SQL Server (LocalDB connection string in `appsettings.json`)
- Run: `dotnet run --project src/InsuranceDMS.API` from `InsuranceDMS/backend/`
- Swagger UI available at `/swagger` on startup
- Database is auto-migrated and seeded on first run

**Frontend** — `InsuranceDMS/frontend/insurance-dms/`
- Angular 17, standalone components, signals, lazy-loaded feature routes
- Requires Node.js 18+ and Angular CLI 17+
- Run: `npm install && ng serve` from `InsuranceDMS/frontend/insurance-dms/`
- Proxies API calls to the backend at `http://localhost:5000`
