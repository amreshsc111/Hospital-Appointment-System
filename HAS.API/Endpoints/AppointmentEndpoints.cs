using HAS.Application.Appointment.Commands;
using HAS.Application.Appointment.Queries;
using HAS.Application.Common.Interfaces;
using HAS.Domain.Enums;
using HAS.Infrastructure.Identity;

namespace HAS.API.Endpoints;

public static class AppointmentEndpoints
{
    public static IEndpointRouteBuilder MapAppointmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/appointments").WithTags("Appointments");

        // Create Appointment (Authenticated)
        group.MapPost("/", async (CreateAppointmentCommand command, IMediator mediator) =>
        {
            // ...
        })
        .WithName("CreateAppointment")
        .RequireAuthorization()
        .WithOpenApi();

        // Update Appointment (Authenticated)
        group.MapPut("/{id:guid}", async (Guid id, UpdateAppointmentCommand command, IMediator mediator) =>
        {
            // ...
        })
        .WithName("UpdateAppointment")
        .RequireAuthorization()
        .WithOpenApi();

        // Cancel Appointment (Authenticated)
        group.MapPost("/{id:guid}/cancel", async (Guid id, CancelAppointmentRequest request, IMediator mediator) =>
        {
            // ...
        })
        .WithName("CancelAppointment")
        .RequireAuthorization()
        .WithOpenApi();

        // Confirm Appointment (Doctor or Receptionist)
        group.MapPost("/{id:guid}/confirm", async (Guid id, IMediator mediator) =>
        {
            // ...
        })
        .WithName("ConfirmAppointment")
        .RequireAuthorization(AuthorizationPolicies.DoctorOrReceptionist)
        .WithOpenApi();

        // Complete Appointment (Doctor only)
        group.MapPost("/{id:guid}/complete", async (Guid id, CompleteAppointmentRequest request, IMediator mediator) =>
        {
            // ...
        })
        .WithName("CompleteAppointment")
        .RequireAuthorization(AuthorizationPolicies.DoctorOnly)
        .WithOpenApi();

        // Mark No-Show (Doctor or Receptionist)
        group.MapPost("/{id:guid}/no-show", async (Guid id, IMediator mediator) =>
        {
            // ...
        })
        .WithName("MarkNoShow")
        .RequireAuthorization(AuthorizationPolicies.DoctorOrReceptionist)
        .WithOpenApi();

        // Get Appointment by ID (Authenticated)
        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            // ...
        })
        .WithName("GetAppointment")
        .RequireAuthorization()
        .WithOpenApi();

        // List Appointments (Authenticated)
        group.MapGet("/", async ([AsParameters] ListAppointmentsQuery query, IMediator mediator) =>
        {
            // ...
        })
        .WithName("ListAppointments")
        .RequireAuthorization()
        .WithOpenApi();

        // Get Available Slots (Authenticated)
        group.MapGet("/doctors/{doctorId:guid}/available-slots", async (
            Guid doctorId,
            DateTime date,
            int slotDurationMinutes,
            IMediator mediator) =>
        {
            // ...
        })
        .WithName("GetAvailableSlots")
        .RequireAuthorization()
        .WithOpenApi();

        return app;
    }
}

// Request DTOs for endpoints that need request bodies
public record CancelAppointmentRequest(string? CancellationReason);
public record CompleteAppointmentRequest(string? CompletionNotes);
