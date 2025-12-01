using HAS.Application.Common.Interfaces;

namespace HAS.Application.DoctorLeave.Commands;

public record ApproveDoctorLeaveCommand(Guid Id) : IRequest<bool>;

public class ApproveDoctorLeaveHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ApproveDoctorLeaveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<bool> Handle(ApproveDoctorLeaveCommand request, CancellationToken cancellationToken)
    {
        var leave = await _unitOfWork.DoctorLeaves.GetByIdAsync(request.Id, cancellationToken);
        if (leave == null)
            throw new Exception($"Leave request with ID '{request.Id}' not found");

        leave.IsApproved = true;

        await _unitOfWork.DoctorLeaves.UpdateAsync(leave, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
