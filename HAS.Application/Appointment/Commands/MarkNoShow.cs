using HAS.Application.Common.Interfaces;
using HAS.Domain.Enums;

namespace HAS.Application.Appointment.Commands;

public record MarkNoShowCommand(Guid Id) : IRequest<bool>;

public class MarkNoShowHandler(IAppointmentRepository appointments) 
    : IRequestHandler<MarkNoShowCommand, bool>
{
    private readonly IAppointmentRepository _appointments = appointments;

    public async Task<bool> Handle(MarkNoShowCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointments.GetByIdAsync(request.Id, cancellationToken);
        if (appointment == null)
            throw new Exception($"Appointment with ID '{request.Id}' not found");

        if (appointment.Status != AppointmentStatus.Confirmed)
            throw new Exception($"Only confirmed appointments can be marked as no-show. Current status: {appointment.Status}");

        appointment.Status = AppointmentStatus.NoShow;

        await _appointments.UpdateAsync(appointment, cancellationToken);
        await _appointments.SaveChangesAsync(cancellationToken);

        return true;
    }
}
