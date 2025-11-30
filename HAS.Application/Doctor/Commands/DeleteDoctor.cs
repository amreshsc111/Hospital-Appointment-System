using HAS.Application.Common.Interfaces;

namespace HAS.Application.Doctor.Commands;

public record DeleteDoctorCommand(Guid Id) : IRequest<bool>;

public class DeleteDoctorHandler(IDoctorRepository doctors) 
    : IRequestHandler<DeleteDoctorCommand, bool>
{
    private readonly IDoctorRepository _doctors = doctors;

    public async Task<bool> Handle(DeleteDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _doctors.GetByIdAsync(request.Id, cancellationToken);
        if (doctor == null)
            throw new Exception($"Doctor with ID '{request.Id}' not found");

        await _doctors.DeleteAsync(request.Id, cancellationToken);
        await _doctors.SaveChangesAsync(cancellationToken);

        return true;
    }
}
