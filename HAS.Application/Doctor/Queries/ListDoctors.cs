using HAS.Application.Common.Interfaces;

using HAS.Application.Common.Models;

namespace HAS.Application.Doctor.Queries;

public record ListDoctorsQuery(Guid? DepartmentId = null, bool? IsAvailable = null) : PaginationQuery, IRequest<PaginatedList<DoctorListItem>>;

public record DoctorListItem(
    Guid Id,
    string FullName,
    string Qualification,
    string DepartmentName,
    string Email,
    string PhoneNumber,
    bool IsAvailable
);

public class ListDoctorsHandler(IDoctorRepository doctors) 
    : IRequestHandler<ListDoctorsQuery, PaginatedList<DoctorListItem>>
{
    private readonly IDoctorRepository _doctors = doctors;

    public async Task<PaginatedList<DoctorListItem>> Handle(ListDoctorsQuery request, CancellationToken cancellationToken)
    {
        List<Domain.Entities.Doctor> doctors;

        if (request.DepartmentId.HasValue)
        {
            doctors = await _doctors.GetByDepartmentIdAsync(request.DepartmentId.Value, cancellationToken);
        }
        else if (request.IsAvailable == true)
        {
            doctors = await _doctors.GetAvailableDoctorsAsync(cancellationToken);
        }
        else
        {
            doctors = await _doctors.GetAllAsync(cancellationToken);
        }

        var items = doctors.Select(d => new DoctorListItem(
            d.Id,
            d.FullName,
            d.Qualification,
            d.Department?.Name ?? "Unknown",
            d.Email.Value,
            d.PhoneNumber.Value,
            d.IsAvailable
        )).ToList();

        // Apply availability filter if specified and not already filtered
        if (request.IsAvailable.HasValue && !request.DepartmentId.HasValue)
        {
            items = items.Where(d => d.IsAvailable == request.IsAvailable.Value).ToList();
        }

        return PaginatedList<DoctorListItem>.Create(items, request.PageNumber, request.PageSize);
    }
}
