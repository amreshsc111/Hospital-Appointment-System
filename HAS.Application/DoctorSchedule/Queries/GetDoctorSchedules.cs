using HAS.Application.Common.Interfaces;

namespace HAS.Application.DoctorSchedule.Queries;

public record GetDoctorSchedulesQuery(Guid DoctorId) : IRequest<List<DoctorScheduleItem>>;

public record DoctorScheduleItem(Guid Id, DayOfWeek DayOfWeek, TimeSpan StartTime, TimeSpan EndTime, int SlotDurationMinutes);

public class GetDoctorSchedulesHandler(IDoctorScheduleRepository schedules)
    : IRequestHandler<GetDoctorSchedulesQuery, List<DoctorScheduleItem>>
{
    private readonly IDoctorScheduleRepository _schedules = schedules;

    public async Task<List<DoctorScheduleItem>> Handle(GetDoctorSchedulesQuery request, CancellationToken cancellationToken)
    {
        var schedules = await _schedules.GetByDoctorIdAsync(request.DoctorId, cancellationToken);
        
        return schedules.Select(s => new DoctorScheduleItem(
            s.Id,
            s.DayOfWeek,
            s.StartTime,
            s.EndTime,
            s.SlotDurationMinutes
        )).ToList();
    }
}
