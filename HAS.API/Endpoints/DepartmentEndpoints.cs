using HAS.Application.Common.Interfaces;
using HAS.Application.Department.Commands;
using HAS.Application.Department.Queries;
using HAS.Infrastructure.Identity;

namespace HAS.API.Endpoints;

public static class DepartmentEndpoints
{
    public static IEndpointRouteBuilder MapDepartmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/departments").WithTags("Departments");

        // Create Department (Admin only)
        group.MapPost("/", async (CreateDepartmentCommand command, IMediator mediator) =>
        {
            // ...
        })
        .WithName("CreateDepartment")
        .RequireAuthorization(AuthorizationPolicies.AdminOnly)
        .WithOpenApi();

        // Update Department (Admin only)
        group.MapPut("/{id:guid}", async (Guid id, UpdateDepartmentCommand command, IMediator mediator) =>
        {
            // ...
        })
        .WithName("UpdateDepartment")
        .RequireAuthorization(AuthorizationPolicies.AdminOnly)
        .WithOpenApi();

        // Delete Department (Admin only)
        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            // ...
        })
        .WithName("DeleteDepartment")
        .RequireAuthorization(AuthorizationPolicies.AdminOnly)
        .WithOpenApi();

        // Get Department by ID (Authenticated)
        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            // ...
        })
        .WithName("GetDepartment")
        .RequireAuthorization()
        .WithOpenApi();

        // List all Departments (Authenticated)
        group.MapGet("/", async ([AsParameters] ListDepartmentsQuery query, IMediator mediator) =>
        {
            // ...
        })
        .WithName("ListDepartments")
        .RequireAuthorization()
        .WithOpenApi();

        return app;
    }
}
