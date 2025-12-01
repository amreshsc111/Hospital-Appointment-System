using FluentValidation;
using HAS.Application.Common.Interfaces;
using HAS.Domain.Enums;
using HAS.Domain.ValueObjects;

namespace HAS.Application.Patient.Commands;

public record UpdatePatientCommand(
    Guid Id,
    string FullName,
    DateTime DateOfBirth,
    Gender Gender,
    string? Address,
    string Email,
    string PhoneNumber
) : IRequest<UpdatePatientResponse>;

public record UpdatePatientResponse(Guid Id, string FullName, string Email);

public class UpdatePatientHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<UpdatePatientCommand, UpdatePatientResponse>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<UpdatePatientResponse> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await _unitOfWork.Patients.GetByIdAsync(request.Id, cancellationToken);
        if (patient == null)
            throw new Exception($"Patient with ID '{request.Id}' not found");

        patient.FullName = request.FullName;
        patient.DateOfBirth = request.DateOfBirth;
        patient.Gender = request.Gender;
        patient.Address = request.Address;
        patient.Email = new Email(request.Email);
        patient.PhoneNumber = new PhoneNumber(request.PhoneNumber);

        await _unitOfWork.Patients.UpdateAsync(patient, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdatePatientResponse(patient.Id, patient.FullName, patient.Email.Value);
    }
}

public class UpdatePatientValidator : AbstractValidator<UpdatePatientCommand>
{
    public UpdatePatientValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Patient ID is required");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .LessThan(DateTime.UtcNow).WithMessage("Date of birth must be in the past");

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Invalid gender value");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required");

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Address must not exceed 500 characters");
    }
}
