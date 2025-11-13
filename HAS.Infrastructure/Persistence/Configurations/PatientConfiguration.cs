using HAS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HAS.Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");
        builder.HasKey(p => p.Id);

        builder.OwnsOne(p => p.Email, e =>
        {
            e.Property(x => x.Value)
                .HasColumnName("Email")
                .HasMaxLength(150)
                .IsRequired();
        });

        builder.OwnsOne(p => p.Phone, pnum =>
        {
            pnum.Property(x => x.Value)
                .HasColumnName("Phone")
                .HasMaxLength(20);
        });

        builder.Property(p => p.FullName).HasMaxLength(150).IsRequired();
        builder.Property(p => p.DateOfBirth).IsRequired();
    }
}
