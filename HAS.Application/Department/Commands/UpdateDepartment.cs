using FluentValidation;
using HAS.Application.Common.Interfaces;

namespace HAS.Application.Department.Commands;

public record UpdateDepartmentCommand(Guid Id, string Name, string? Description) : IRequest<UpdateDepartmentResponse>;
public record UpdateDepartmentResponse(Guid Id, string Name, string? Description);

public class UpdateDepartmentHandler(IDepartmentRepository departments) 
    : IRequestHandler<UpdateDepartmentCommand, UpdateDepartmentResponse>
{
    private readonly IDepartmentRepository _departments = departments;

    public async Task<UpdateDepartmentResponse> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await _departments.GetByIdAsync(request.Id, cancellationToken);
        if (department == null)
            throw new Exception($"Department with ID '{request.Id}' not found");

        department.Name = request.Name;
        department.Description = request.Description;

        await _departments.UpdateAsync(department, cancellationToken);
        await _departments.SaveChangesAsync(cancellationToken);

        return new UpdateDepartmentResponse(department.Id, department.Name, department.Description);
    }
}

public class UpdateDepartmentValidator : AbstractValidator<UpdateDepartmentCommand>
{
    public UpdateDepartmentValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Department ID is required");
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Department name is required")
            .MaximumLength(100).WithMessage("Department name must not exceed 100 characters");
        
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");
    }
}
