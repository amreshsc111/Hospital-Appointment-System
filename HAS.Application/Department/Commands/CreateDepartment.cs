using FluentValidation;
using HAS.Application.Common.Interfaces;
using HAS.Domain.Entities;

namespace HAS.Application.Department.Commands;

public record CreateDepartmentCommand(string Name, string? Description) : IRequest<CreateDepartmentResponse>;
public record CreateDepartmentResponse(Guid Id, string Name, string? Description);

public class CreateDepartmentHandler(IDepartmentRepository departments) 
    : IRequestHandler<CreateDepartmentCommand, CreateDepartmentResponse>
{
    private readonly IDepartmentRepository _departments = departments;

    public async Task<CreateDepartmentResponse> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        if (await _departments.ExistsByNameAsync(request.Name, cancellationToken))
            throw new Exception($"Department with name '{request.Name}' already exists");

        var department = new Domain.Entities.Department
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description
        };

        await _departments.AddAsync(department, cancellationToken);
        await _departments.SaveChangesAsync(cancellationToken);

        return new CreateDepartmentResponse(department.Id, department.Name, department.Description);
    }
}

public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Department name is required")
            .MaximumLength(100).WithMessage("Department name must not exceed 100 characters");
        
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");
    }
}
