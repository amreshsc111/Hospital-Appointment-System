using HAS.Application.Common.Interfaces;

namespace HAS.Application.Appointment.Queries;

public record GetAvailableSlotsQuery(
    Guid DoctorId,
    DateTime Date,
    int SlotDurationMinutes = 30
) : IRequest<List<TimeSlot>>;

public record TimeSlot(DateTime StartTime, DateTime EndTime, bool IsAvailable);

public class GetAvailableSlotsHandler(IAppointmentRepository appointments, IDoctorRepository doctors) 
    : IRequestHandler<GetAvailableSlotsQuery, List<TimeSlot>>
{
    private readonly IAppointmentRepository _appointments = appointments;
    private readonly IDoctorRepository _doctors = doctors;

    // Working hours: 9 AM to 5 PM
    private const int WorkingHoursStart = 9;
    private const int WorkingHoursEnd = 17;

    public async Task<List<TimeSlot>> Handle(GetAvailableSlotsQuery request, CancellationToken cancellationToken)
    {
        // Verify doctor exists
        var doctor = await _doctors.GetByIdAsync(request.DoctorId, cancellationToken);
        if (doctor == null)
            throw new Exception($"Doctor with ID '{request.DoctorId}' not found");

        // Get all appointments for the doctor on the specified date
        var appointments = await _appointments.GetByDoctorAndDateAsync(
            request.DoctorId,
            request.Date.Date,
            cancellationToken
        );

        // Filter only confirmed and pending appointments
        var bookedAppointments = appointments
            .Where(a => a.Status == Domain.Enums.AppointmentStatus.Confirmed || 
                       a.Status == Domain.Enums.AppointmentStatus.Pending)
            .ToList();

        // Generate time slots for the day
        var slots = new List<TimeSlot>();
        var currentTime = request.Date.Date.AddHours(WorkingHoursStart);
        var endOfDay = request.Date.Date.AddHours(WorkingHoursEnd);

        while (currentTime < endOfDay)
        {
            var slotEnd = currentTime.AddMinutes(request.SlotDurationMinutes);
            
            // Check if this slot conflicts with any booked appointment
            var isAvailable = !bookedAppointments.Any(a =>
                (currentTime >= a.StartAt && currentTime < a.EndAt) ||
                (slotEnd > a.StartAt && slotEnd <= a.EndAt) ||
                (currentTime <= a.StartAt && slotEnd >= a.EndAt)
            );

            // Don't show past slots
            if (currentTime > DateTime.UtcNow)
            {
                slots.Add(new TimeSlot(currentTime, slotEnd, isAvailable));
            }

            currentTime = slotEnd;
        }

        return slots;
    }
}
