using HAS.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace HAS.Domain.Entities;

public class CancellationPolicy : BaseEntity
{
    public Guid DoctorId { get; set; }
    [ForeignKey(nameof(DoctorId))]
    public Doctor Doctor { get; set; } = default!;
    
    public int MinimumHoursBeforeAppointment { get; set; } = 24;
    public decimal CancellationFeePercentage { get; set; } = 0; // 0-100
    public bool AllowCancellation { get; set; } = true;
    public string? PolicyDescription { get; set; }
}
