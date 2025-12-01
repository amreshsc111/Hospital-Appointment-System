using HAS.Application.Common.Interfaces;
using HAS.Domain.Enums;

namespace HAS.Application.Appointment.Commands;

public record CompleteAppointmentCommand(Guid Id, string? CompletionNotes) : IRequest<bool>;

public class CompleteAppointmentHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<CompleteAppointmentCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<bool> Handle(CompleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.Id, cancellationToken);
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

        await _unitOfWork.Appointments.UpdateAsync(appointment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
