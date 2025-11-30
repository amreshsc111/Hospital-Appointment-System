using HAS.Application.Common.Interfaces;

namespace HAS.Application.Department.Queries;

using HAS.Application.Common.Models;

public record ListDepartmentsQuery : PaginationQuery, IRequest<PaginatedList<DepartmentListItem>>;
public record DepartmentListItem(Guid Id, string Name, string? Description, int DoctorCount);

public class ListDepartmentsHandler(IDepartmentRepository departments) 
    : IRequestHandler<ListDepartmentsQuery, PaginatedList<DepartmentListItem>>
{
    private readonly IDepartmentRepository _departments = departments;

    public async Task<PaginatedList<DepartmentListItem>> Handle(ListDepartmentsQuery request, CancellationToken cancellationToken)
    {
        var departments = await _departments.GetAllAsync(cancellationToken);
        
        var items = departments.Select(d => new DepartmentListItem(
            d.Id,
            d.Name,
            d.Description,
            d.Doctors?.Count ?? 0
        )).ToList();

        return PaginatedList<DepartmentListItem>.Create(items, request.PageNumber, request.PageSize);
    }
}
