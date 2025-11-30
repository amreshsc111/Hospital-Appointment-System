using HAS.Application.Common.Interfaces;
using HAS.Domain.Enums;

namespace HAS.Application.Patient.Queries;

public record GetPatientQuery(Guid Id) : IRequest<GetPatientResponse?>;

public record GetPatientResponse(
    Guid Id,
    string FullName,
    DateTime DateOfBirth,
    int Age,
    Gender Gender,
    string? Address,
    string Email,
    string PhoneNumber,
    DateTime CreatedAt
);

public class GetPatientHandler(IPatientRepository patients) 
    : IRequestHandler<GetPatientQuery, GetPatientResponse?>
{
    private readonly IPatientRepository _patients = patients;

    public async Task<GetPatientResponse?> Handle(GetPatientQuery request, CancellationToken cancellationToken)
    {
        var patient = await _patients.GetByIdAsync(request.Id, cancellationToken);
        if (patient == null)
            return null;

        var age = CalculateAge(patient.DateOfBirth);

        return new GetPatientResponse(
            patient.Id,
            patient.FullName,
            patient.DateOfBirth,
            age,
            patient.Gender,
            patient.Address,
            patient.Email.Value,
            patient.PhoneNumber.Value,
            patient.CreatedAt
        );
    }

    private static int CalculateAge(DateTime dateOfBirth)
    {
        var today = DateTime.UtcNow;
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > today.AddYears(-age)) age--;
        return age;
    }
}
