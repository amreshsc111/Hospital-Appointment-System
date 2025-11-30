using HAS.Application.Common.Interfaces;

namespace HAS.Application.DoctorLeave.Queries;

public record GetDoctorLeavesQuery(Guid DoctorId) : IRequest<List<DoctorLeaveItem>>;

public record DoctorLeaveItem(Guid Id, DateTime StartDate, DateTime EndDate, string Reason, bool IsApproved);

public class GetDoctorLeavesHandler(IDoctorLeaveRepository leaves)
    : IRequestHandler<GetDoctorLeavesQuery, List<DoctorLeaveItem>>
{
    private readonly IDoctorLeaveRepository _leaves = leaves;

    public async Task<List<DoctorLeaveItem>> Handle(GetDoctorLeavesQuery request, CancellationToken cancellationToken)
    {
        var leaves = await _leaves.GetByDoctorIdAsync(request.DoctorId, cancellationToken);
        
        return leaves.Select(l => new DoctorLeaveItem(
            l.Id,
            l.StartDate,
            l.EndDate,
            l.Reason,
            l.IsApproved
        )).ToList();
    }
}
