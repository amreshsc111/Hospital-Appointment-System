using HAS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HAS.Infrastructure.Persistence.Configurations;

public class AppointmentHistoryConfiguration : IEntityTypeConfiguration<AppointmentHistory>
{
    public void Configure(EntityTypeBuilder<AppointmentHistory> builder)
    {
        builder.ToTable("AppointmentHistories");
        builder.HasKey(ah => ah.Id);

        builder.Property(ah => ah.Action).IsRequired().HasMaxLength(50);
        builder.Property(ah => ah.OldStatus).HasConversion<int>();
        builder.Property(ah => ah.NewStatus).HasConversion<int>();
        builder.Property(ah => ah.Notes).HasMaxLength(500);

        builder.HasOne(ah => ah.Appointment)
            .WithMany(a => a.History)
            .HasForeignKey(ah => ah.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
