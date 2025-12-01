using HAS.Application.Common.Interfaces;

namespace HAS.Application.Department.Commands;

public record DeleteDepartmentCommand(Guid Id) : IRequest<bool>;

public class DeleteDepartmentHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<DeleteDepartmentCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<bool> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await _unitOfWork.Departments.GetByIdAsync(request.Id, cancellationToken);
        if (department == null)
            throw new Exception($"Department with ID '{request.Id}' not found");

        await _unitOfWork.Departments.DeleteAsync(request.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
