using HAS.Application.Common.Interfaces;
using HAS.Application.DoctorSchedule.Commands;
using HAS.Application.DoctorSchedule.Queries;
using HAS.Infrastructure.Identity;

namespace HAS.API.Endpoints;

public static class DoctorScheduleEndpoints
{
    public static IEndpointRouteBuilder MapDoctorScheduleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/doctors/{doctorId:guid}/schedules").WithTags("Doctor Schedules");

        // Create Schedule (Doctor or Admin)
        group.MapPost("/", async (Guid doctorId, CreateDoctorScheduleCommand command, IMediator mediator) =>
        {
            try
            {
                if (doctorId != command.DoctorId)
                    return Results.BadRequest(new { error = "Doctor ID mismatch" });

                var result = await mediator.Send(command);
                return Results.Created($"/api/doctors/{doctorId}/schedules/{result.Id}", result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("CreateDoctorSchedule")
        .RequireAuthorization(AuthorizationPolicies.AdminOrDoctor)
        .WithOpenApi();

        // Get Schedules (Authenticated)
        group.MapGet("/", async (Guid doctorId, IMediator mediator) =>
        {
            try
            {
                var result = await mediator.Send(new GetDoctorSchedulesQuery(doctorId));
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("GetDoctorSchedules")
        .RequireAuthorization()
        .WithOpenApi();

        return app;
    }
}
