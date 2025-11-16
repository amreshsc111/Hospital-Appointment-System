# Hospital Appointment System

Lightweight ASP.NET Core Web API for managing hospital appointments, authentication, and persistence. This repository contains the `HAS.API` project (ASP.NET Core 9, C# 13) and supporting infrastructure components.

## Tech stack
- .NET 9 (C# 13)
- ASP.NET Core Web API
- Entity Framework Core (SQL Server)
- JWT Authentication
- Swagger / OpenAPI

## Quick start

Prerequisites
- .NET 9 SDK
- SQL Server (or compatible)
- Optional: __Visual Studio 2026__ or another IDE (VS Code, Rider)

Clone

Restore, build and run

Open API docs (when running in Development):
- Swagger UI: https://localhost:5001/swagger/index.html (port may vary)

## Configuration

Main settings live in `appsettings.json` and environment-specific files (for local dev use `appsettings.Development.json`). Key settings to provide:

- Jwt:Secret — secret key used to sign JWT tokens (do NOT commit secrets)
- ConnectionStrings:DefaultConnection — SQL Server connection string

Example `appsettings.Development.json` (do not commit secrets):

For local secrets use environment variables or __dotnet user-secrets__:

## Database (EF Core)

Add and apply migrations from solution root (adjust startup/project args if needed):

If you prefer, run migrations programmatically or from __Visual Studio__ Package Manager Console.

## Development notes

- Global exception handling middleware is registered in `Program.cs` via `UseGlobalExceptionHandler`.
- JWT auth is configured; make sure `Jwt:Secret` is set before running authenticated endpoints.
- Swagger/OpenAPI is enabled in development.

## Contributing
- Create feature branches from `master`.
- Run tests and ensure builds succeed before submitting PR.
- Sensitive data should never be committed. Add secrets to `appsettings.Development.json` or use user-secrets.