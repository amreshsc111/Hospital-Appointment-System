using HAS.Application.Common.Interfaces;
using HAS.Domain.Enums;

namespace HAS.Application.Appointment.Queries;

public record GetAppointmentQuery(Guid Id) : IRequest<GetAppointmentResponse?>;

public record GetAppointmentResponse(
    Guid Id,
    Guid DoctorId,
    string DoctorName,
    string DepartmentName,
    Guid PatientId,
    string PatientName,
    DateTime StartAt,
    DateTime EndAt,
    AppointmentStatus Status,
    string Reason,
    string? Notes,
    DateTime CreatedAt
);

public class GetAppointmentHandler(IAppointmentRepository appointments) 
    : IRequestHandler<GetAppointmentQuery, GetAppointmentResponse?>
{
    private readonly IAppointmentRepository _appointments = appointments;

    public async Task<GetAppointmentResponse?> Handle(GetAppointmentQuery request, CancellationToken cancellationToken)
    {
        var appointment = await _appointments.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (appointment == null)
            return null;

        return new GetAppointmentResponse(
            appointment.Id,
            appointment.DoctorId,
            appointment.Doctor?.FullName ?? "Unknown",
            appointment.Doctor?.Department?.Name ?? "Unknown",
            appointment.PatientId,
            appointment.Patient?.FullName ?? "Unknown",
            appointment.StartAt,
            appointment.EndAt,
            appointment.Status,
            appointment.Reason,
            appointment.Notes,
            appointment.CreatedAt
        );
    }
}
