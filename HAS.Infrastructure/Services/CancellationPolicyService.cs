using HAS.Application.Common.Interfaces;

namespace HAS.Infrastructure.Services;

public class CancellationPolicyService(
    IAppointmentRepository appointments,
    ICancellationPolicyRepository policies) : ICancellationPolicyService
{
    private readonly IAppointmentRepository _appointments = appointments;
    private readonly ICancellationPolicyRepository _policies = policies;

    public async Task<(bool CanCancel, string? Reason, decimal Fee)> ValidateCancellationAsync(
        Guid appointmentId,
        CancellationToken cancellationToken)
    {
        var appointment = await _appointments.GetByIdAsync(appointmentId, cancellationToken);
        if (appointment == null)
            return (false, "Appointment not found", 0);

        // Get doctor's cancellation policy
        var policy = await _policies.GetByDoctorIdAsync(appointment.DoctorId, cancellationToken);

        // If no policy exists, use default (allow cancellation with no fee)
        if (policy == null)
        {
            return (true, null, 0);
        }

        // Check if cancellation is allowed
        if (!policy.AllowCancellation)
        {
            return (false, "Cancellation is not allowed for this doctor", 0);
        }

        // Calculate hours until appointment
        var hoursUntilAppointment = (appointment.StartAt - DateTime.UtcNow).TotalHours;

        // Check if within minimum hours requirement
        if (hoursUntilAppointment < policy.MinimumHoursBeforeAppointment)
        {
            var fee = policy.CancellationFeePercentage;
            var reason = $"Cancellation must be made at least {policy.MinimumHoursBeforeAppointment} hours before the appointment. " +
                        $"A cancellation fee of {fee}% may apply.";
            return (true, reason, fee);
        }

        return (true, null, 0);
    }
}
