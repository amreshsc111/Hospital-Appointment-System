using HAS.Domain.Common;
using HAS.Domain.Enums;

namespace HAS.Domain.Entities;

public class Appointment : BaseEntity
{
    public Guid DoctorId { get; set; }
    public Guid PatientId { get; set; }
    public DateTime StartAt { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? Notes { get; set; }
}
