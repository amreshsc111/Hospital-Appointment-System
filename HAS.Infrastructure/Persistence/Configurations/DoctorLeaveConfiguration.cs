using HAS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HAS.Infrastructure.Persistence.Configurations;

public class DoctorLeaveConfiguration : IEntityTypeConfiguration<DoctorLeave>
{
    public void Configure(EntityTypeBuilder<DoctorLeave> builder)
    {
        builder.ToTable("DoctorLeaves");
        builder.HasKey(dl => dl.Id);

        builder.Property(dl => dl.StartDate).IsRequired();
        builder.Property(dl => dl.EndDate).IsRequired();
        builder.Property(dl => dl.Reason).IsRequired().HasMaxLength(500);

        builder.HasOne(dl => dl.Doctor)
            .WithMany(d => d.Leaves)
            .HasForeignKey(dl => dl.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
