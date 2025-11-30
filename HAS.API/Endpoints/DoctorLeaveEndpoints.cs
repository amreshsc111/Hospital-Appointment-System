using HAS.Application.Common.Interfaces;
using HAS.Application.DoctorLeave.Commands;
using HAS.Application.DoctorLeave.Queries;
using HAS.Infrastructure.Identity;

namespace HAS.API.Endpoints;

public static class DoctorLeaveEndpoints
{
    public static IEndpointRouteBuilder MapDoctorLeaveEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/doctors").WithTags("Doctor Leaves");

        // Create Leave Request (Doctor only)
        group.MapPost("/{doctorId:guid}/leaves", async (Guid doctorId, CreateDoctorLeaveCommand command, IMediator mediator) =>
        {
            try
            {
                if (doctorId != command.DoctorId)
                    return Results.BadRequest(new { error = "Doctor ID mismatch" });

                var result = await mediator.Send(command);
                return Results.Created($"/api/doctors/{doctorId}/leaves/{result.Id}", result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("CreateDoctorLeave")
        .RequireAuthorization(AuthorizationPolicies.DoctorOnly)
        .WithOpenApi();

        // Approve Leave (Admin only)
        group.MapPost("/leaves/{id:guid}/approve", async (Guid id, IMediator mediator) =>
        {
            try
            {
                var result = await mediator.Send(new ApproveDoctorLeaveCommand(id));
                return Results.Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("ApproveDoctorLeave")
        .RequireAuthorization(AuthorizationPolicies.AdminOnly)
        .WithOpenApi();

        // Get Leaves (Doctor or Admin)
        group.MapGet("/{doctorId:guid}/leaves", async (Guid doctorId, IMediator mediator) =>
        {
            try
            {
                var result = await mediator.Send(new GetDoctorLeavesQuery(doctorId));
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("GetDoctorLeaves")
        .RequireAuthorization(AuthorizationPolicies.AdminOrDoctor)
        .WithOpenApi();

        return app;
    }
}
