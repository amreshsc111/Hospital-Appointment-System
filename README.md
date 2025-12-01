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

# HAS.API

Minimal API project (ASP.NET Core .NET 9) for the HAS system.

## Overview
HAS.API is a minimal Web API using:
- .NET 9
- Minimal APIs
- Entity Framework Core (SQL Server)
- MediatR for request/command handling
- JWT Authentication
- Swagger / OpenAPI
- Custom middleware (global exception handler)

## Requirements
- .NET 9 SDK
- Visual Studio 2026 or VS Code
- SQL Server instance accessible from `ConnectionStrings:DefaultConnection`

## Quickstart

1. Clone the repository:
   - git clone <repository-url>

2. Configure settings
   - Update `appsettings.Development.json` (or `appsettings.json`) with:
     - `ConnectionStrings:DefaultConnection`
     - `Jwt:Secret`

   Example:

3. Restore and build:
- `dotnet restore`
- `dotnet build`

4. Database migrations
- Add or apply migrations from the project containing the `ApplicationDbContext`:
  - `dotnet ef migrations add InitialCreate --project HAS.Infrastructure --startup-project HAS.API`
  - `dotnet ef database update --project HAS.Infrastructure --startup-project HAS.API`

5. Run
- `dotnet run --project HAS.API`
- Or debug via Visual Studio 2026.

6. Explore API
- Swagger (when `ASPNETCORE_ENVIRONMENT=Development`) at `https://localhost:<port>/swagger`.

## Important implementation notes
- `Program.cs` wires up services including `AddInfrastructureServices()`, JWT auth, EF Core DbContext, OpenAPI, and the endpoints:
- `MapAuthLogin()`, `MapRegister()`, `MapRefreshToken()`
- Minimal API endpoint parameters:
- Parameters are inferred from route, query, body, or DI.
- If an endpoint accepts `IMediator` (or other services), ensure the service is registered in DI (e.g., with MediatR) or mark the parameter with `[FromServices]`.

## Common issue and fix
- Exception:
````````
- Cause: An endpoint parameter (e.g., `mediator`) could not be resolved by DI because MediatR/`IMediator` was not registered or the parameter type is not a registered service.
- Fixes:
  - Register MediatR:
    ```csharp
    // Program.cs
    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));
    ```
  - Or mark parameter explicitly:
    ```csharp
    app.MapPost("/login", async (LoginCommand command, [FromServices] IMediator mediator) => { ... });
    ```

## Troubleshooting
- Verify `IMediator` is registered in DI.
- Ensure the assembly containing handlers is included in MediatR registration.
- Confirm connection string and DB migrations are applied.
- Check `Jwt:Secret` is set for authentication endpoints.

## Contributing
- Follow repository branch and PR guidelines.
- Run tests and linters before creating a PR.

