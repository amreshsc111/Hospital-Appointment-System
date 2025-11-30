using HAS.Application.Common.Interfaces;
using HAS.Domain.Enums;

namespace HAS.Application.Appointment.Commands;

public record CompleteAppointmentCommand(Guid Id, string? CompletionNotes) : IRequest<bool>;

public class CompleteAppointmentHandler(IAppointmentRepository appointments) 
    : IRequestHandler<CompleteAppointmentCommand, bool>
{
    private readonly IAppointmentRepository _appointments = appointments;

    public async Task<bool> Handle(CompleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointments.GetByIdAsync(request.Id, cancellationToken);
        if (appointment == null)
            throw new Exception($"Appointment with ID '{request.Id}' not found");

        if (appointment.Status != AppointmentStatus.Confirmed)
            throw new Exception($"Only confirmed appointments can be completed. Current status: {appointment.Status}");

        appointment.Status = AppointmentStatus.Completed;
        if (!string.IsNullOrWhiteSpace(request.CompletionNotes))
        {
            appointment.Notes = string.IsNullOrWhiteSpace(appointment.Notes)
                ? $"Completed: {request.CompletionNotes}"
                : $"{appointment.Notes}\nCompleted: {request.CompletionNotes}";
        }

        await _appointments.UpdateAsync(appointment, cancellationToken);
        await _appointments.SaveChangesAsync(cancellationToken);

        return true;
    }
}
