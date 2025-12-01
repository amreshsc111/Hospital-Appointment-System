using HAS.Application.Common.Interfaces;

namespace HAS.Application.Doctor.Commands;

public record DeleteDoctorCommand(Guid Id) : IRequest<bool>;

public class DeleteDoctorHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<DeleteDoctorCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<bool> Handle(DeleteDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(request.Id, cancellationToken);
        if (doctor == null)
            throw new Exception($"Doctor with ID '{request.Id}' not found");

        await _unitOfWork.Doctors.DeleteAsync(request.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
