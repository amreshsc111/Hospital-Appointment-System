using HAS.Application.Common.Interfaces;

namespace HAS.Application.Patient.Commands;

public record DeletePatientCommand(Guid Id) : IRequest<bool>;

public class DeletePatientHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<DeletePatientCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<bool> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await _unitOfWork.Patients.GetByIdAsync(request.Id, cancellationToken);
        if (patient == null)
            throw new Exception($"Patient with ID '{request.Id}' not found");

        await _unitOfWork.Patients.DeleteAsync(request.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
