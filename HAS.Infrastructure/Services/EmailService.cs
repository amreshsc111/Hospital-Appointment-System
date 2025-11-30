using HAS.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace HAS.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
        _smtpHost = configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
        _smtpPort = int.Parse(configuration["Email:SmtpPort"] ?? "587");
        _smtpUsername = configuration["Email:Username"] ?? throw new InvalidOperationException("Email:Username not configured");
        _smtpPassword = configuration["Email:Password"] ?? throw new InvalidOperationException("Email:Password not configured");
        _fromEmail = configuration["Email:FromEmail"] ?? _smtpUsername;
        _fromName = configuration["Email:FromName"] ?? "Hospital Appointment System";
    }

    public async Task SendAppointmentReminderAsync(
        string toEmail,
        string patientName,
        string doctorName,
        DateTime appointmentTime,
        string departmentName,
        CancellationToken cancellationToken = default)
    {
        var subject = "Appointment Reminder - Hospital Appointment System";
        var body = $@"
            <html>
            <body>
                <h2>Appointment Reminder</h2>
                <p>Dear {patientName},</p>
                <p>This is a reminder for your upcoming appointment:</p>
                <ul>
                    <li><strong>Doctor:</strong> {doctorName}</li>
                    <li><strong>Department:</strong> {departmentName}</li>
                    <li><strong>Date & Time:</strong> {appointmentTime:dddd, MMMM dd, yyyy 'at' hh:mm tt}</li>
                </ul>
                <p>Please arrive 10 minutes early for check-in.</p>
                <p>If you need to cancel or reschedule, please contact us as soon as possible.</p>
                <br/>
                <p>Best regards,<br/>Hospital Appointment System</p>
            </body>
            </html>";

        await SendEmailAsync(toEmail, subject, body, cancellationToken);
    }

    public async Task SendAppointmentConfirmationAsync(
        string toEmail,
        string patientName,
        string doctorName,
        DateTime appointmentTime,
        string departmentName,
        CancellationToken cancellationToken = default)
    {
        var subject = "Appointment Confirmation - Hospital Appointment System";
        var body = $@"
            <html>
            <body>
                <h2>Appointment Confirmed</h2>
                <p>Dear {patientName},</p>
                <p>Your appointment has been confirmed:</p>
                <ul>
                    <li><strong>Doctor:</strong> {doctorName}</li>
                    <li><strong>Department:</strong> {departmentName}</li>
                    <li><strong>Date & Time:</strong> {appointmentTime:dddd, MMMM dd, yyyy 'at' hh:mm tt}</li>
                </ul>
                <p>Please arrive 10 minutes early for check-in.</p>
                <br/>
                <p>Best regards,<br/>Hospital Appointment System</p>
            </body>
            </html>";

        await SendEmailAsync(toEmail, subject, body, cancellationToken);
    }

    public async Task SendAppointmentCancellationAsync(
        string toEmail,
        string patientName,
        string doctorName,
        DateTime appointmentTime,
        string reason,
        CancellationToken cancellationToken = default)
    {
        var subject = "Appointment Cancelled - Hospital Appointment System";
        var body = $@"
            <html>
            <body>
                <h2>Appointment Cancelled</h2>
                <p>Dear {patientName},</p>
                <p>Your appointment has been cancelled:</p>
                <ul>
                    <li><strong>Doctor:</strong> {doctorName}</li>
                    <li><strong>Date & Time:</strong> {appointmentTime:dddd, MMMM dd, yyyy 'at' hh:mm tt}</li>
                    <li><strong>Reason:</strong> {reason}</li>
                </ul>
                <p>If you would like to reschedule, please contact us.</p>
                <br/>
                <p>Best regards,<br/>Hospital Appointment System</p>
            </body>
            </html>";

        await SendEmailAsync(toEmail, subject, body, cancellationToken);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken)
    {
        using var smtpClient = new SmtpClient(_smtpHost, _smtpPort)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(_smtpUsername, _smtpPassword)
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_fromEmail, _fromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmail);

        await smtpClient.SendMailAsync(mailMessage, cancellationToken);
    }
}
