using HAS.Application.CancellationPolicy.Commands;
using HAS.Application.Common.Interfaces;
using HAS.Infrastructure.Identity;

namespace HAS.API.Endpoints;

public static class CancellationPolicyEndpoints
{
    public static IEndpointRouteBuilder MapCancellationPolicyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/doctors/{doctorId:guid}/cancellation-policy").WithTags("Cancellation Policies");

        // Create/Update Policy (Doctor or Admin)
        group.MapPost("/", async (Guid doctorId, CreateCancellationPolicyCommand command, IMediator mediator) =>
        {
            try
            {
                if (doctorId != command.DoctorId)
                    return Results.BadRequest(new { error = "Doctor ID mismatch" });

                var result = await mediator.Send(command);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("SetCancellationPolicy")
        .RequireAuthorization(AuthorizationPolicies.AdminOrDoctor)
        .WithOpenApi();

        return app;
    }
}
