using FluentValidation;
using HAS.Application.Common.Interfaces;
using HAS.Domain.Enums;

namespace HAS.Application.Appointment.Commands;

public record UpdateAppointmentCommand(
    Guid Id,
    DateTime StartAt,
    DateTime EndAt,
    string Reason,
    string? Notes
) : IRequest<UpdateAppointmentResponse>;

public record UpdateAppointmentResponse(Guid Id, DateTime StartAt, DateTime EndAt, AppointmentStatus Status);

public class UpdateAppointmentHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<UpdateAppointmentCommand, UpdateAppointmentResponse>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<UpdateAppointmentResponse> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.Id, cancellationToken);
        if (appointment == null)
            throw new Exception($"Appointment with ID '{request.Id}' not found");

        // Only allow updating pending or confirmed appointments
        if (appointment.Status == AppointmentStatus.Completed || 
            appointment.Status == AppointmentStatus.Cancelled ||
            appointment.Status == AppointmentStatus.NoShow)
            throw new Exception($"Cannot update appointment with status '{appointment.Status}'");

        // Check for conflicts with the new time slot
        var hasConflict = await _unitOfWork.Appointments.HasConflictAsync(
            appointment.DoctorId,
            request.StartAt,
            request.EndAt,
            appointment.Id,
            cancellationToken
        );

        if (hasConflict)
            throw new Exception($"Doctor already has an appointment during this time slot");

        appointment.StartAt = request.StartAt;
        appointment.EndAt = request.EndAt;
        appointment.Reason = request.Reason;
        appointment.Notes = request.Notes;

        await _unitOfWork.Appointments.UpdateAsync(appointment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateAppointmentResponse(appointment.Id, appointment.StartAt, appointment.EndAt, appointment.Status);
    }
}

public class UpdateAppointmentValidator : AbstractValidator<UpdateAppointmentCommand>
{
    public UpdateAppointmentValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Appointment ID is required");

        RuleFor(x => x.StartAt)
            .NotEmpty().WithMessage("Start time is required")
            .GreaterThan(DateTime.UtcNow).WithMessage("Start time must be in the future");

        RuleFor(x => x.EndAt)
            .NotEmpty().WithMessage("End time is required")
            .GreaterThan(x => x.StartAt).WithMessage("End time must be after start time");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required")
            .MaximumLength(500).WithMessage("Reason must not exceed 500 characters");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes must not exceed 1000 characters");
    }
}
