using HAS.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace HAS.Domain.Entities;

public class DoctorLeave : BaseEntity
{
    public Guid DoctorId { get; set; }
    [ForeignKey(nameof(DoctorId))]
    public Doctor Doctor { get; set; } = default!;
    
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = default!;
    public bool IsApproved { get; set; } = false;
}
