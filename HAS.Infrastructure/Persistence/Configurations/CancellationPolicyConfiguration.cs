using HAS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HAS.Infrastructure.Persistence.Configurations;

public class CancellationPolicyConfiguration : IEntityTypeConfiguration<CancellationPolicy>
{
    public void Configure(EntityTypeBuilder<CancellationPolicy> builder)
    {
        builder.ToTable("CancellationPolicies");
        builder.HasKey(cp => cp.Id);

        builder.Property(cp => cp.MinimumHoursBeforeAppointment).IsRequired();
        builder.Property(cp => cp.CancellationFeePercentage).HasPrecision(5, 2);
        builder.Property(cp => cp.PolicyDescription).HasMaxLength(1000);

        builder.HasOne(cp => cp.Doctor)
            .WithOne(d => d.CancellationPolicy)
            .HasForeignKey<CancellationPolicy>(cp => cp.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
