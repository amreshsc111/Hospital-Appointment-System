using HAS.Application.Common.Interfaces;

namespace HAS.Application.Appointment.Queries;

public record GetAppointmentHistoryQuery(Guid AppointmentId) : IRequest<List<AppointmentHistoryItem>>;

public record AppointmentHistoryItem(
    DateTime ChangedAt,
    string Action,
    string OldStatus,
    string NewStatus,
    DateTime? OldStartAt,
    DateTime? NewStartAt,
    string? Notes
);

public class GetAppointmentHistoryHandler(IAppointmentHistoryRepository history)
    : IRequestHandler<GetAppointmentHistoryQuery, List<AppointmentHistoryItem>>
{
    private readonly IAppointmentHistoryRepository _history = history;

    public async Task<List<AppointmentHistoryItem>> Handle(GetAppointmentHistoryQuery request, CancellationToken cancellationToken)
    {
        var history = await _history.GetByAppointmentIdAsync(request.AppointmentId, cancellationToken);
        
        return history.Select(h => new AppointmentHistoryItem(
            h.CreatedAt,
            h.Action,
            h.OldStatus.ToString(),
            h.NewStatus.ToString(),
            h.OldStartAt,
            h.NewStartAt,
            h.Notes
        )).ToList();
    }
}
