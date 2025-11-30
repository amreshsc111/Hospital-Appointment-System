using FluentValidation;
using HAS.Application.Common.Interfaces;
using HAS.Domain.Entities;

namespace HAS.Application.CancellationPolicy.Commands;

public record CreateCancellationPolicyCommand(
    Guid DoctorId,
    int MinimumHoursBeforeAppointment,
    decimal CancellationFeePercentage,
    bool AllowCancellation = true,
    string? PolicyDescription = null
) : IRequest<CreateCancellationPolicyResponse>;

public record CreateCancellationPolicyResponse(Guid Id, int MinimumHours, decimal FeePercentage);

public class CreateCancellationPolicyHandler(ICancellationPolicyRepository policies, IDoctorRepository doctors)
    : IRequestHandler<CreateCancellationPolicyCommand, CreateCancellationPolicyResponse>
{
    private readonly ICancellationPolicyRepository _policies = policies;
    private readonly IDoctorRepository _doctors = doctors;

    public async Task<CreateCancellationPolicyResponse> Handle(CreateCancellationPolicyCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _doctors.GetByIdAsync(request.DoctorId, cancellationToken);
        if (doctor == null)
            throw new Exception($"Doctor with ID '{request.DoctorId}' not found");

        // Check if policy already exists
        var existing = await _policies.GetByDoctorIdAsync(request.DoctorId, cancellationToken);
        if (existing != null)
            throw new Exception($"Cancellation policy already exists for this doctor");

        var policy = new Domain.Entities.CancellationPolicy
        {
            Id = Guid.NewGuid(),
            DoctorId = request.DoctorId,
            MinimumHoursBeforeAppointment = request.MinimumHoursBeforeAppointment,
            CancellationFeePercentage = request.CancellationFeePercentage,
            AllowCancellation = request.AllowCancellation,
            PolicyDescription = request.PolicyDescription
        };

        await _policies.AddAsync(policy, cancellationToken);
        await _policies.SaveChangesAsync(cancellationToken);

        return new CreateCancellationPolicyResponse(policy.Id, policy.MinimumHoursBeforeAppointment, policy.CancellationFeePercentage);
    }
}

public class CreateCancellationPolicyValidator : AbstractValidator<CreateCancellationPolicyCommand>
{
    public CreateCancellationPolicyValidator()
    {
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x.MinimumHoursBeforeAppointment).GreaterThanOrEqualTo(0).LessThanOrEqualTo(168); // Max 1 week
        RuleFor(x => x.CancellationFeePercentage).GreaterThanOrEqualTo(0).LessThanOrEqualTo(100);
    }
}
