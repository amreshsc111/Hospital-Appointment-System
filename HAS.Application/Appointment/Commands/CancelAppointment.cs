using HAS.Application.Common.Interfaces;
using HAS.Domain.Entities;
using HAS.Domain.Enums;

namespace HAS.Application.Appointment.Commands;

public record CancelAppointmentCommand(Guid Id, string? CancellationReason) : IRequest<CancelAppointmentResponse>;
public record CancelAppointmentResponse(bool Success, string? Message, decimal CancellationFee);

public class CancelAppointmentHandler(
    IUnitOfWork unitOfWork,
    ICancellationPolicyService policyService,
    IEmailService emailService)
    : IRequestHandler<CancelAppointmentCommand, CancelAppointmentResponse>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICancellationPolicyService _policyService = policyService;
    private readonly IEmailService _emailService = emailService;

    public async Task<CancelAppointmentResponse> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (appointment == null)
            throw new Exception($"Appointment with ID '{request.Id}' not found");

        if (appointment.Status == AppointmentStatus.Cancelled)
            throw new Exception("Appointment is already cancelled");

        if (appointment.Status == AppointmentStatus.Completed)
            throw new Exception("Cannot cancel a completed appointment");

        // Validate cancellation policy
        var (canCancel, policyReason, fee) = await _policyService.ValidateCancellationAsync(request.Id, cancellationToken);
        
        if (!canCancel)
            throw new Exception(policyReason ?? "Cancellation not allowed");

        var oldStatus = appointment.Status;
        appointment.Status = AppointmentStatus.Cancelled;
        
        if (!string.IsNullOrWhiteSpace(request.CancellationReason))
        {
            appointment.Notes = string.IsNullOrWhiteSpace(appointment.Notes)
                ? $"Cancelled: {request.CancellationReason}"
                : $"{appointment.Notes}\nCancelled: {request.CancellationReason}";
        }

        await _unitOfWork.Appointments.UpdateAsync(appointment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Add history record
        var historyRecord = new AppointmentHistory
        {
            Id = Guid.NewGuid(),
            AppointmentId = appointment.Id,
            OldStatus = oldStatus,
            NewStatus = AppointmentStatus.Cancelled,
            Action = "Cancelled",
            Notes = request.CancellationReason,
            ChangedByUserId = Guid.Empty // TODO: Get from current user service
        };
        await _unitOfWork.AppointmentHistories.AddAsync(historyRecord, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send cancellation email
        try
        {
            await _emailService.SendAppointmentCancellationAsync(
                appointment.Patient.Email.Value,
                appointment.Patient.FullName,
                appointment.Doctor.FullName,
                appointment.StartAt,
                request.CancellationReason ?? "No reason provided",
                cancellationToken
            );
        }
        catch
        {
            // Log email failure but don't fail the cancellation
        }

        return new CancelAppointmentResponse(true, policyReason, fee);
    }
}
