using HAS.Application.Common.Interfaces;

namespace HAS.Application.Doctor.Queries;

public record GetDoctorQuery(Guid Id) : IRequest<GetDoctorResponse?>;

public record GetDoctorResponse(
    Guid Id,
    string FullName,
    string Qualification,
    Guid DepartmentId,
    string DepartmentName,
    string Email,
    string PhoneNumber,
    bool IsAvailable,
    DateTime CreatedAt
);

public class GetDoctorHandler(IDoctorRepository doctors) 
    : IRequestHandler<GetDoctorQuery, GetDoctorResponse?>
{
    private readonly IDoctorRepository _doctors = doctors;

    public async Task<GetDoctorResponse?> Handle(GetDoctorQuery request, CancellationToken cancellationToken)
    {
        var doctor = await _doctors.GetByIdWithDepartmentAsync(request.Id, cancellationToken);
        if (doctor == null)
            return null;

        return new GetDoctorResponse(
            doctor.Id,
            doctor.FullName,
            doctor.Qualification,
            doctor.DepartmentId,
            doctor.Department?.Name ?? "Unknown",
            doctor.Email.Value,
            doctor.PhoneNumber.Value,
            doctor.IsAvailable,
            doctor.CreatedAt
        );
    }
}
