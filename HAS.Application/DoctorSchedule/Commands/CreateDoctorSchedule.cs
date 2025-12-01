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

public class CreateDoctorScheduleHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateDoctorScheduleCommand, CreateDoctorScheduleResponse>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<CreateDoctorScheduleResponse> Handle(CreateDoctorScheduleCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(request.DoctorId, cancellationToken);
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

        await _unitOfWork.DoctorSchedules.AddAsync(schedule, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
