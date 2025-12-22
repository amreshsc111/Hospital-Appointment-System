using HAS.Domain.Common;
using HAS.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace HAS.Domain.Entities;

public class Appointment : BaseEntity
{
    public Guid DoctorId { get; set; }
    [ForeignKey(nameof(DoctorId))]
    public Doctor Doctor { get; set; } = default!;
    
    public Guid PatientId { get; set; }
    [ForeignKey(nameof(PatientId))]
    public Patient Patient { get; set; } = default!;
    
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public AppointmentStatus Status { get; set; }
    public string Reason { get; set; } = default!;
    public string? Notes { get; set; }
    
    public ICollection<AppointmentHistory> History { get; set; } = [];
    public ICollection<AppointmentReminder> Reminders { get; set; } = [];
}
