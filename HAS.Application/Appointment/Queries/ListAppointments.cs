using HAS.Application.Common.Interfaces;
using HAS.Domain.Enums;

using HAS.Application.Common.Models;

namespace HAS.Application.Appointment.Queries;

public record ListAppointmentsQuery(
    Guid? DoctorId = null,
    Guid? PatientId = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    AppointmentStatus? Status = null
) : PaginationQuery, IRequest<PaginatedList<AppointmentListItem>>;

public record AppointmentListItem(
    Guid Id,
    string DoctorName,
    string PatientName,
    DateTime StartAt,
    DateTime EndAt,
    AppointmentStatus Status,
    string Reason
);

public class ListAppointmentsHandler(IAppointmentRepository appointments) 
    : IRequestHandler<ListAppointmentsQuery, PaginatedList<AppointmentListItem>>
{
    private readonly IAppointmentRepository _appointments = appointments;

    public async Task<PaginatedList<AppointmentListItem>> Handle(ListAppointmentsQuery request, CancellationToken cancellationToken)
    {
        List<Domain.Entities.Appointment> appointmentList;

        if (request.DoctorId.HasValue)
        {
            appointmentList = await _appointments.GetByDoctorIdAsync(request.DoctorId.Value, cancellationToken);
        }
        else if (request.PatientId.HasValue)
        {
            appointmentList = await _appointments.GetByPatientIdAsync(request.PatientId.Value, cancellationToken);
        }
        else if (request.StartDate.HasValue && request.EndDate.HasValue)
        {
            appointmentList = await _appointments.GetByDateRangeAsync(request.StartDate.Value, request.EndDate.Value, cancellationToken);
        }
        else if (request.Status.HasValue)
        {
            appointmentList = await _appointments.GetByStatusAsync(request.Status.Value, cancellationToken);
        }
        else
        {
            appointmentList = await _appointments.GetAllAsync(cancellationToken);
        }

        var items = appointmentList.Select(a => new AppointmentListItem(
            a.Id,
            a.Doctor?.FullName ?? "Unknown",
            a.Patient?.FullName ?? "Unknown",
            a.StartAt,
            a.EndAt,
            a.Status,
            a.Reason
        )).ToList();

        // Apply additional filters if needed
        if (request.Status.HasValue && !request.DoctorId.HasValue && !request.PatientId.HasValue && !request.StartDate.HasValue)
        {
            items = items.Where(a => a.Status == request.Status.Value).ToList();
        }

        items = items.OrderBy(a => a.StartAt).ToList();

        return PaginatedList<AppointmentListItem>.Create(items, request.PageNumber, request.PageSize);
    }
}
