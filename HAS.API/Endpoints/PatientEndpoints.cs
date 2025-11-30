using HAS.Application.Common.Interfaces;
using HAS.Application.Patient.Commands;
using HAS.Application.Patient.Queries;
using HAS.Infrastructure.Identity;

namespace HAS.API.Endpoints;

public static class PatientEndpoints
{
    public static IEndpointRouteBuilder MapPatientEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/patients").WithTags("Patients");

        // Create Patient (Admin or Receptionist)
        group.MapPost("/", async (CreatePatientCommand command, IMediator mediator) =>
        {
            // ...
        })
        .WithName("CreatePatient")
        .RequireAuthorization(AuthorizationPolicies.AdminOrReceptionist)
        .WithOpenApi();

        // Update Patient (Admin or Receptionist)
        group.MapPut("/{id:guid}", async (Guid id, UpdatePatientCommand command, IMediator mediator) =>
        {
            // ...
        })
        .WithName("UpdatePatient")
        .RequireAuthorization(AuthorizationPolicies.AdminOrReceptionist)
        .WithOpenApi();

        // Delete Patient (Admin only)
        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            // ...
        })
        .WithName("DeletePatient")
        .RequireAuthorization(AuthorizationPolicies.AdminOnly)
        .WithOpenApi();

        // Get Patient by ID (Authenticated)
        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            // ...
        })
        .WithName("GetPatient")
        .RequireAuthorization()
        .WithOpenApi();

        // List all Patients (Admin, Doctor, Receptionist)
        group.MapGet("/", async ([AsParameters] ListPatientsQuery query, IMediator mediator) =>
        {
            // ...
        })
        .WithName("ListPatients")
        .RequireAuthorization(AuthorizationPolicies.AdminOrDoctor) // Or Receptionist - need to update policy constant
        .WithOpenApi();

        return app;
    }
}
