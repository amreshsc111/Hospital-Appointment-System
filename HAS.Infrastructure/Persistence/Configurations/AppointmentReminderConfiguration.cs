using HAS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HAS.Infrastructure.Persistence.Configurations;

public class AppointmentReminderConfiguration : IEntityTypeConfiguration<AppointmentReminder>
{
    public void Configure(EntityTypeBuilder<AppointmentReminder> builder)
    {
        builder.ToTable("AppointmentReminders");
        builder.HasKey(ar => ar.Id);

        builder.Property(ar => ar.ScheduledFor).IsRequired();
        builder.Property(ar => ar.Status).HasConversion<int>();

        builder.HasOne(ar => ar.Appointment)
            .WithMany(a => a.Reminders)
            .HasForeignKey(ar => ar.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
