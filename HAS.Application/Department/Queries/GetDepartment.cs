using HAS.Application.Common.Interfaces;

namespace HAS.Application.Department.Queries;

public record GetDepartmentQuery(Guid Id) : IRequest<GetDepartmentResponse?>;
public record GetDepartmentResponse(Guid Id, string Name, string? Description, DateTime CreatedAt);

public class GetDepartmentHandler(IDepartmentRepository departments) 
    : IRequestHandler<GetDepartmentQuery, GetDepartmentResponse?>
{
    private readonly IDepartmentRepository _departments = departments;

    public async Task<GetDepartmentResponse?> Handle(GetDepartmentQuery request, CancellationToken cancellationToken)
    {
        var department = await _departments.GetByIdAsync(request.Id, cancellationToken);
        if (department == null)
            return null;

        return new GetDepartmentResponse(
            department.Id,
            department.Name,
            department.Description,
            department.CreatedAt
        );
    }
}
