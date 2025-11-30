using HAS.Application.Common.Interfaces;

namespace HAS.Application.Department.Commands;

public record DeleteDepartmentCommand(Guid Id) : IRequest<bool>;

public class DeleteDepartmentHandler(IDepartmentRepository departments) 
    : IRequestHandler<DeleteDepartmentCommand, bool>
{
    private readonly IDepartmentRepository _departments = departments;

    public async Task<bool> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await _departments.GetByIdAsync(request.Id, cancellationToken);
        if (department == null)
            throw new Exception($"Department with ID '{request.Id}' not found");

        await _departments.DeleteAsync(request.Id, cancellationToken);
        await _departments.SaveChangesAsync(cancellationToken);

        return true;
    }
}
