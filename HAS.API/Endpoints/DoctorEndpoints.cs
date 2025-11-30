using HAS.Application.Common.Interfaces;
using HAS.Application.Doctor.Commands;
using HAS.Application.Doctor.Queries;
using HAS.Infrastructure.Identity;

namespace HAS.API.Endpoints;

public static class DoctorEndpoints
{
    public static IEndpointRouteBuilder MapDoctorEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/doctors").WithTags("Doctors");

        // Create Doctor (Admin only)
        group.MapPost("/", async (CreateDoctorCommand command, IMediator mediator) =>
        {
            // ...
        })
        .WithName("CreateDoctor")
        .RequireAuthorization(AuthorizationPolicies.AdminOnly)
        .WithOpenApi();

        // Update Doctor (Admin only)
        group.MapPut("/{id:guid}", async (Guid id, UpdateDoctorCommand command, IMediator mediator) =>
        {
            // ...
        })
        .WithName("UpdateDoctor")
        .RequireAuthorization(AuthorizationPolicies.AdminOnly)
        .WithOpenApi();

        // Delete Doctor (Admin only)
        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            // ...
        })
        .WithName("DeleteDoctor")
        .RequireAuthorization(AuthorizationPolicies.AdminOnly)
        .WithOpenApi();

        // Get Doctor by ID (Authenticated)
        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            // ...
        })
        .WithName("GetDoctor")
        .RequireAuthorization()
        .WithOpenApi();

        // List Doctors (Authenticated)
        group.MapGet("/", async ([AsParameters] ListDoctorsQuery query, IMediator mediator) =>
        {
            // ...
        })
        .WithName("ListDoctors")
        .RequireAuthorization()
        .WithOpenApi();

        return app;
    }
}
