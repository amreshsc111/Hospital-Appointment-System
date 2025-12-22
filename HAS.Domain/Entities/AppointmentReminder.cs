using HAS.Domain.Common;
using HAS.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace HAS.Domain.Entities;

public class AppointmentReminder : BaseEntity
{
    public Guid AppointmentId { get; set; }
    [ForeignKey(nameof(AppointmentId))]
    public Appointment Appointment { get; set; } = default!;
    
    public DateTime ScheduledFor { get; set; }
    public DateTime? SentAt { get; set; }
    public ReminderStatus Status { get; set; } = ReminderStatus.Pending;
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; } = 0;
}
