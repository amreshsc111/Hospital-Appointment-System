using HAS.Application.Appointment.Queries;
using HAS.Application.Common.Interfaces;
using HAS.Infrastructure.Identity;

namespace HAS.API.Endpoints;

public static class AppointmentHistoryEndpoints
{
    public static IEndpointRouteBuilder MapAppointmentHistoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/appointments/{id:guid}/history").WithTags("Appointment History");

        // Get History (Authenticated)
        group.MapGet("/", async (Guid id, IMediator mediator) =>
        {
            try
            {
                var result = await mediator.Send(new GetAppointmentHistoryQuery(id));
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("GetAppointmentHistory")
        .RequireAuthorization()
        .WithOpenApi();

        return app;
    }
}
