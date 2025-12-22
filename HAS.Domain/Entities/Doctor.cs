using HAS.Domain.Common;
using HAS.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace HAS.Domain.Entities;

public class Doctor : BaseEntity
{
    public string FullName { get; set; } = default!;
    public string Qualification { get; set; } = default!;
    
    public Guid DepartmentId { get; set; }
    [ForeignKey(nameof(DepartmentId))]
    public Department Department { get; set; } = default!;
    
    public Email Email { get; set; } = default!;
    public PhoneNumber PhoneNumber { get; set; } = default!;
    public bool IsAvailable { get; set; } = true;
    
    public ICollection<Appointment> Appointments { get; set; } = [];
    public ICollection<DoctorSchedule> Schedules { get; set; } = [];
    public ICollection<DoctorLeave> Leaves { get; set; } = [];
    public CancellationPolicy? CancellationPolicy { get; set; }
}