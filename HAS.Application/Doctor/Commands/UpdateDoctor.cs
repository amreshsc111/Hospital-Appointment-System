using FluentValidation;
using HAS.Application.Common.Interfaces;
using HAS.Domain.ValueObjects;

namespace HAS.Application.Doctor.Commands;

public record UpdateDoctorCommand(
    Guid Id,
    string FullName,
    string Qualification,
    Guid DepartmentId,
    string Email,
    string PhoneNumber,
    bool IsAvailable
) : IRequest<UpdateDoctorResponse>;

public record UpdateDoctorResponse(Guid Id, string FullName, string Email);

public class UpdateDoctorHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<UpdateDoctorCommand, UpdateDoctorResponse>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<UpdateDoctorResponse> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(request.Id, cancellationToken);
        if (doctor == null)
            throw new Exception($"Doctor with ID '{request.Id}' not found");

        // Verify department exists
        var department = await _unitOfWork.Departments.GetByIdAsync(request.DepartmentId, cancellationToken);
        if (department == null)
            throw new Exception($"Department with ID '{request.DepartmentId}' not found");

        doctor.FullName = request.FullName;
        doctor.Qualification = request.Qualification;
        doctor.DepartmentId = request.DepartmentId;
        doctor.Email = new Email(request.Email);
        doctor.PhoneNumber = new PhoneNumber(request.PhoneNumber);
        doctor.IsAvailable = request.IsAvailable;

        await _unitOfWork.Doctors.UpdateAsync(doctor, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateDoctorResponse(doctor.Id, doctor.FullName, doctor.Email.Value);
    }
}

public class UpdateDoctorValidator : AbstractValidator<UpdateDoctorCommand>
{
    public UpdateDoctorValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Doctor ID is required");

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
