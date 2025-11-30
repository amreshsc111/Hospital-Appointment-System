using HAS.Application.Common.Interfaces;
using HAS.Domain.Enums;

namespace HAS.Application.Patient.Queries;

using HAS.Application.Common.Models;

public record ListPatientsQuery : PaginationQuery, IRequest<PaginatedList<PatientListItem>>;

public record PatientListItem(
    Guid Id,
    string FullName,
    int Age,
    Gender Gender,
    string Email,
    string PhoneNumber
);

public class ListPatientsHandler(IPatientRepository patients) 
    : IRequestHandler<ListPatientsQuery, PaginatedList<PatientListItem>>
{
    private readonly IPatientRepository _patients = patients;

    public async Task<PaginatedList<PatientListItem>> Handle(ListPatientsQuery request, CancellationToken cancellationToken)
    {
        var patients = await _patients.GetAllAsync(cancellationToken);
        
        var items = patients.Select(p => new PatientListItem(
            p.Id,
            p.FullName,
            CalculateAge(p.DateOfBirth),
            p.Gender,
            p.Email.Value,
            p.PhoneNumber.Value
        )).ToList();

        return PaginatedList<PatientListItem>.Create(items, request.PageNumber, request.PageSize);
    }

    private static int CalculateAge(DateTime dateOfBirth)
    {
        var today = DateTime.UtcNow;
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > today.AddYears(-age)) age--;
        return age;
    }
}
