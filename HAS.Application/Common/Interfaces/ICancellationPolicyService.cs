namespace HAS.Application.Common.Interfaces;

public interface ICancellationPolicyService
{
    Task<(bool CanCancel, string? Reason, decimal Fee)> ValidateCancellationAsync(
        Guid appointmentId,
        CancellationToken cancellationToken);
}
