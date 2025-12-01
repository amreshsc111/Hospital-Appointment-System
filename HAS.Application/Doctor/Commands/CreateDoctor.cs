using FluentValidation;
using HAS.Application.Common.Interfaces;
using HAS.Domain.Entities;
using HAS.Domain.ValueObjects;

namespace HAS.Application.Doctor.Commands;

public record CreateDoctorCommand(
    string FullName,
    string Qualification,
    Guid DepartmentId,
    string Email,
    string PhoneNumber,
    bool IsAvailable = true
) : IRequest<CreateDoctorResponse>;

public record CreateDoctorResponse(Guid Id, string FullName, string Email);

public class CreateDoctorHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<CreateDoctorCommand, CreateDoctorResponse>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<CreateDoctorResponse> Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
    {
        // Verify department exists
        var department = await _unitOfWork.Departments.GetByIdAsync(request.DepartmentId, cancellationToken);
        if (department == null)
            throw new Exception($"Department with ID '{request.DepartmentId}' not found");

        // Check for duplicate email
        if (await _unitOfWork.Doctors.ExistsByEmailAsync(request.Email, cancellationToken))
            throw new Exception($"Doctor with email '{request.Email}' already exists");

        var doctor = new Domain.Entities.Doctor
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Qualification = request.Qualification,
            DepartmentId = request.DepartmentId,
            Email = new Email(request.Email),
            PhoneNumber = new PhoneNumber(request.PhoneNumber),
            IsAvailable = request.IsAvailable
        };

        await _unitOfWork.Doctors.AddAsync(doctor, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateDoctorResponse(doctor.Id, doctor.FullName, doctor.Email.Value);
    }
}

public class CreateDoctorValidator : AbstractValidator<CreateDoctorCommand>
{
    public CreateDoctorValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters");

        RuleFor(x => x.Qualification)
            .NotEmpty().WithMessage("Qualification is required")
            .MaximumLength(200).WithMessage("Qualification must not exceed 200 characters");

        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Department ID is required");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required");
    }
}
