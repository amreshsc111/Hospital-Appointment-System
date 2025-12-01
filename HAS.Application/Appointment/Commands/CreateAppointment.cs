using FluentValidation;
using HAS.Application.Common.Interfaces;
using HAS.Domain.Entities;
using HAS.Domain.Enums;

namespace HAS.Application.Appointment.Commands;

public record CreateAppointmentCommand(
    Guid DoctorId,
    Guid PatientId,
    DateTime StartAt,
    DateTime EndAt,
    string Reason,
    string? Notes
) : IRequest<CreateAppointmentResponse>;

public record CreateAppointmentResponse(Guid Id, DateTime StartAt, DateTime EndAt, AppointmentStatus Status);

public class CreateAppointmentHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<CreateAppointmentCommand, CreateAppointmentResponse>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<CreateAppointmentResponse> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        // Verify doctor exists and is available
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(request.DoctorId, cancellationToken);
        if (doctor == null)
            throw new Exception($"Doctor with ID '{request.DoctorId}' not found");

        if (!doctor.IsAvailable)
            throw new Exception($"Doctor '{doctor.FullName}' is currently not available");

        // Verify patient exists
        var patient = await _unitOfWork.Patients.GetByIdAsync(request.PatientId, cancellationToken);
        if (patient == null)
            throw new Exception($"Patient with ID '{request.PatientId}' not found");

        // Check for appointment conflicts
        var hasConflict = await _unitOfWork.Appointments.HasConflictAsync(
            request.DoctorId,
            request.StartAt,
            request.EndAt,
            null,
            cancellationToken
        );

        if (hasConflict)
            throw new Exception($"Doctor already has an appointment during this time slot");

        var appointment = new Domain.Entities.Appointment
        {
            Id = Guid.NewGuid(),
            DoctorId = request.DoctorId,
            PatientId = request.PatientId,
            StartAt = request.StartAt,
            EndAt = request.EndAt,
            Status = AppointmentStatus.Pending,
            Reason = request.Reason,
            Notes = request.Notes
        };

        await _unitOfWork.Appointments.AddAsync(appointment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Create reminder (6 hours before by default)
        var reminderTime = appointment.StartAt.AddHours(-6);
        if (reminderTime > DateTime.UtcNow)
        {
            var reminder = new Domain.Entities.AppointmentReminder
            {
                Id = Guid.NewGuid(),
                AppointmentId = appointment.Id,
                ScheduledFor = reminderTime,
                Status = Domain.Enums.ReminderStatus.Pending
            };
            await _unitOfWork.AppointmentReminders.AddAsync(reminder, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Add history record
        var historyRecord = new Domain.Entities.AppointmentHistory
        {
            Id = Guid.NewGuid(),
            AppointmentId = appointment.Id,
            OldStatus = AppointmentStatus.Pending,
            NewStatus = AppointmentStatus.Pending,
            Action = "Created",
            Notes = "Appointment created",
            ChangedByUserId = Guid.Empty
        };
        await _unitOfWork.AppointmentHistories.AddAsync(historyRecord, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateAppointmentResponse(appointment.Id, appointment.StartAt, appointment.EndAt, appointment.Status);
    }
}

public class CreateAppointmentValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentValidator()
    {
        RuleFor(x => x.DoctorId)
            .NotEmpty().WithMessage("Doctor ID is required");

        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Patient ID is required");

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
