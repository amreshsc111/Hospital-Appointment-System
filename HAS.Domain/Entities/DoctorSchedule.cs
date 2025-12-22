using HAS.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace HAS.Domain.Entities;

public class DoctorSchedule : BaseEntity
{
    public Guid DoctorId { get; set; }
    [ForeignKey(nameof(DoctorId))]
    public Doctor Doctor { get; set; } = default!;
    
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsAvailable { get; set; } = true;
    public int SlotDurationMinutes { get; set; } = 30;
}
