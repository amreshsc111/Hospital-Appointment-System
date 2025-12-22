using HAS.Domain.Common;
using HAS.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace HAS.Domain.Entities;

public class AppointmentHistory : BaseEntity
{
    public Guid AppointmentId { get; set; }
    [ForeignKey(nameof(AppointmentId))]
    public Appointment Appointment { get; set; } = default!;
    
    public AppointmentStatus OldStatus { get; set; }
    public AppointmentStatus NewStatus { get; set; }
    
    public DateTime? OldStartAt { get; set; }
    public DateTime? NewStartAt { get; set; }
    
    public DateTime? OldEndAt { get; set; }
    public DateTime? NewEndAt { get; set; }
    
    public string Action { get; set; } = default!; // Created, Updated, Cancelled, Confirmed, Completed, NoShow
    public string? Notes { get; set; }
    public Guid ChangedByUserId { get; set; }
}
