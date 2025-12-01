using HAS.Application.Common.Interfaces;
using HAS.Domain.Enums;

namespace HAS.Application.Appointment.Commands;

public record MarkNoShowCommand(Guid Id) : IRequest<bool>;

public class MarkNoShowHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<MarkNoShowCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<bool> Handle(MarkNoShowCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.Id, cancellationToken);
        if (appointment == null)
            throw new Exception($"Appointment with ID '{request.Id}' not found");

        if (appointment.Status != AppointmentStatus.Confirmed)
            throw new Exception($"Only confirmed appointments can be marked as no-show. Current status: {appointment.Status}");

        appointment.Status = AppointmentStatus.NoShow;

        await _unitOfWork.Appointments.UpdateAsync(appointment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
