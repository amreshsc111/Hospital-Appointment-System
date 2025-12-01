using FluentValidation;
using HAS.Application.Common.Interfaces;
using HAS.Domain.Entities;
using HAS.Domain.Enums;
using HAS.Domain.ValueObjects;

namespace HAS.Application.Patient.Commands;

public record CreatePatientCommand(
    string FullName,
    DateTime DateOfBirth,
    Gender Gender,
    string? Address,
    string Email,
    string PhoneNumber
) : IRequest<CreatePatientResponse>;

public record CreatePatientResponse(Guid Id, string FullName, string Email);

public class CreatePatientHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<CreatePatientCommand, CreatePatientResponse>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<CreatePatientResponse> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        // Check for duplicate email
        if (await _unitOfWork.Patients.ExistsByEmailAsync(request.Email, cancellationToken))
            throw new Exception($"Patient with email '{request.Email}' already exists");

        var patient = new Domain.Entities.Patient
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Address = request.Address,
            Email = new Email(request.Email),
            PhoneNumber = new PhoneNumber(request.PhoneNumber)
        };

        await _unitOfWork.Patients.AddAsync(patient, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreatePatientResponse(patient.Id, patient.FullName, patient.Email.Value);
    }
}

public class CreatePatientValidator : AbstractValidator<CreatePatientCommand>
{
    public CreatePatientValidator()
    {
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
