using HAS.Application.Common.Interfaces;

namespace HAS.Application.DoctorLeave.Commands;

public record ApproveDoctorLeaveCommand(Guid Id) : IRequest<bool>;

public class ApproveDoctorLeaveHandler(IDoctorLeaveRepository leaves)
    : IRequestHandler<ApproveDoctorLeaveCommand, bool>
{
    private readonly IDoctorLeaveRepository _leaves = leaves;

    public async Task<bool> Handle(ApproveDoctorLeaveCommand request, CancellationToken cancellationToken)
    {
        var leave = await _leaves.GetByIdAsync(request.Id, cancellationToken);
        if (leave == null)
            throw new Exception($"Leave request with ID '{request.Id}' not found");

        leave.IsApproved = true;

        await _leaves.UpdateAsync(leave, cancellationToken);
        await _leaves.SaveChangesAsync(cancellationToken);

        return true;
    }
}
