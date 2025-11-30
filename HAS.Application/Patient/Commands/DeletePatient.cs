using HAS.Application.Common.Interfaces;

namespace HAS.Application.Patient.Commands;

public record DeletePatientCommand(Guid Id) : IRequest<bool>;

public class DeletePatientHandler(IPatientRepository patients) 
    : IRequestHandler<DeletePatientCommand, bool>
{
    private readonly IPatientRepository _patients = patients;

    public async Task<bool> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await _patients.GetByIdAsync(request.Id, cancellationToken);
        if (patient == null)
            throw new Exception($"Patient with ID '{request.Id}' not found");

        await _patients.DeleteAsync(request.Id, cancellationToken);
        await _patients.SaveChangesAsync(cancellationToken);

        return true;
    }
}
