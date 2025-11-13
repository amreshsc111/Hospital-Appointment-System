using HAS.Domain.Common;
using HAS.Domain.Enums;

namespace HAS.Domain.Entities;

public class Appointment : BaseEntity
{
    public Guid PatientId { get; private set; }
    public Guid DoctorId { get; private set; }
    public DateTime StartAt { get; private set; }
    public DateTime? EndAt { get; private set; }
    public AppointmentStatus Status { get; private set; } = AppointmentStatus.Pending;
    public string? Notes { get; private set; }


    private Appointment() { }

    public Appointment(Guid patientId, Guid doctorId, DateTime startAt, DateTime? endAt = null, string? notes = null)
    {
        if (patientId == Guid.Empty) throw new ArgumentException("PatientId is required", nameof(patientId));
        if (doctorId == Guid.Empty) throw new ArgumentException("DoctorId is required", nameof(doctorId));
        if (startAt <= DateTime.UtcNow.AddMinutes(-1)) throw new ArgumentException("Start time must be in the future", nameof(startAt));


        PatientId = patientId;
        DoctorId = doctorId;
        StartAt = startAt;
        EndAt = endAt;
        Notes = notes;
    }

    public void Confirm() { Status = AppointmentStatus.Confirmed; TouchModified(); }
    public void Cancel(string? reason = null) { Status = AppointmentStatus.Cancelled; Notes = reason ?? Notes; TouchModified(); }
    public void MarkCompleted() { Status = AppointmentStatus.Completed; TouchModified(); }
    public void NoShow() { Status = AppointmentStatus.NoShow; TouchModified(); }
    public void Reschedule(DateTime newStartAt, DateTime? newEndAt = null)
    {
        if (newStartAt <= DateTime.UtcNow.AddMinutes(-1)) throw new ArgumentException("New start time must be in the future", nameof(newStartAt));
        StartAt = newStartAt;
        EndAt = newEndAt;
        TouchModified();
    }
}
