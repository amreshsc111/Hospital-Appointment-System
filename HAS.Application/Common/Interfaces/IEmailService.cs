namespace HAS.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendAppointmentReminderAsync(
        string toEmail,
        string patientName,
        string doctorName,
        DateTime appointmentTime,
        string departmentName,
        CancellationToken cancellationToken = default);

    Task SendAppointmentConfirmationAsync(
        string toEmail,
        string patientName,
        string doctorName,
        DateTime appointmentTime,
        string departmentName,
        CancellationToken cancellationToken = default);

    Task SendAppointmentCancellationAsync(
        string toEmail,
        string patientName,
        string doctorName,
        DateTime appointmentTime,
        string reason,
        CancellationToken cancellationToken = default);
}
