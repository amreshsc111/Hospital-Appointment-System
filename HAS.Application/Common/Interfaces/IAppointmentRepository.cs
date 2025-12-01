using HAS.Domain.Entities;
using HAS.Domain.Enums;

namespace HAS.Application.Common.Interfaces;

public interface IAppointmentRepository
{
    Task<Domain.Entities.Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Domain.Entities.Appointment?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Domain.Entities.Appointment>> GetAllAsync(CancellationToken cancellationToken);
    Task<List<Domain.Entities.Appointment>> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken);
    Task<List<Domain.Entities.Appointment>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken);
    Task<List<Domain.Entities.Appointment>> GetByDoctorAndDateAsync(Guid doctorId, DateTime date, CancellationToken cancellationToken);
    Task<List<Domain.Entities.Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
    Task<List<Domain.Entities.Appointment>> GetByStatusAsync(AppointmentStatus status, CancellationToken cancellationToken);
    Task<bool> HasConflictAsync(Guid doctorId, DateTime startAt, DateTime endAt, Guid? excludeAppointmentId, CancellationToken cancellationToken);
    Task AddAsync(Domain.Entities.Appointment appointment, CancellationToken cancellationToken);
    Task UpdateAsync(Domain.Entities.Appointment appointment, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
