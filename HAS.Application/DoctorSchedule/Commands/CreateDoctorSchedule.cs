using FluentValidation;
using HAS.Application.Common.Interfaces;
using HAS.Domain.Entities;

namespace HAS.Application.DoctorSchedule.Commands;

public record CreateDoctorScheduleCommand(
    Guid DoctorId,
    DayOfWeek DayOfWeek,
    TimeSpan StartTime,
    TimeSpan EndTime,
    int SlotDurationMinutes = 30
) : IRequest<CreateDoctorScheduleResponse>;

public record CreateDoctorScheduleResponse(Guid Id, DayOfWeek DayOfWeek, TimeSpan StartTime, TimeSpan EndTime);

public class CreateDoctorScheduleHandler(IDoctorScheduleRepository schedules, IDoctorRepository doctors)
    : IRequestHandler<CreateDoctorScheduleCommand, CreateDoctorScheduleResponse>
{
    private readonly IDoctorScheduleRepository _schedules = schedules;
    private readonly IDoctorRepository _doctors = doctors;

    public async Task<CreateDoctorScheduleResponse> Handle(CreateDoctorScheduleCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _doctors.GetByIdAsync(request.DoctorId, cancellationToken);
        if (doctor == null)
            throw new Exception($"Doctor with ID '{request.DoctorId}' not found");

        var schedule = new Domain.Entities.DoctorSchedule
        {
            Id = Guid.NewGuid(),
            DoctorId = request.DoctorId,
            DayOfWeek = request.DayOfWeek,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            SlotDurationMinutes = request.SlotDurationMinutes,
            IsAvailable = true
        };

        await _schedules.AddAsync(schedule, cancellationToken);
        await _schedules.SaveChangesAsync(cancellationToken);

        return new CreateDoctorScheduleResponse(schedule.Id, schedule.DayOfWeek, schedule.StartTime, schedule.EndTime);
    }
}

public class CreateDoctorScheduleValidator : AbstractValidator<CreateDoctorScheduleCommand>
{
    public CreateDoctorScheduleValidator()
    {
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x.StartTime).LessThan(x => x.EndTime).WithMessage("Start time must be before end time");
        RuleFor(x => x.SlotDurationMinutes).GreaterThan(0).LessThanOrEqualTo(120);
    }
}
