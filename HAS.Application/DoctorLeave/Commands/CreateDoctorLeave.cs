using FluentValidation;
using HAS.Application.Common.Interfaces;
using HAS.Domain.Entities;

namespace HAS.Application.DoctorLeave.Commands;

public record CreateDoctorLeaveCommand(
    Guid DoctorId,
    DateTime StartDate,
    DateTime EndDate,
    string Reason
) : IRequest<CreateDoctorLeaveResponse>;

public record CreateDoctorLeaveResponse(Guid Id, DateTime StartDate, DateTime EndDate, bool IsApproved);

public class CreateDoctorLeaveHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateDoctorLeaveCommand, CreateDoctorLeaveResponse>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<CreateDoctorLeaveResponse> Handle(CreateDoctorLeaveCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(request.DoctorId, cancellationToken);
        if (doctor == null)
            throw new Exception($"Doctor with ID '{request.DoctorId}' not found");

        var leave = new Domain.Entities.DoctorLeave
        {
            Id = Guid.NewGuid(),
            DoctorId = request.DoctorId,
            StartDate = request.StartDate.Date,
            EndDate = request.EndDate.Date,
            Reason = request.Reason,
            IsApproved = false
        };

        await _unitOfWork.DoctorLeaves.AddAsync(leave, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateDoctorLeaveResponse(leave.Id, leave.StartDate, leave.EndDate, leave.IsApproved);
    }
}

public class CreateDoctorLeaveValidator : AbstractValidator<CreateDoctorLeaveCommand>
{
    public CreateDoctorLeaveValidator()
    {
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x.StartDate).GreaterThanOrEqualTo(DateTime.Today);
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate);
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}
