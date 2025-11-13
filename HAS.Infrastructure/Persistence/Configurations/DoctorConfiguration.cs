using HAS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HAS.Infrastructure.Persistence.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("Doctors");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.FullName).HasMaxLength(150).IsRequired();
        builder.Property(d => d.Qualification).HasMaxLength(100);

        builder.OwnsOne(d => d.Email, e =>
        {
            e.Property(x => x.Value).HasColumnName("Email").HasMaxLength(150);
        });

        builder.OwnsOne(d => d.Phone, p =>
        {
            p.Property(x => x.Value).HasColumnName("Phone").HasMaxLength(20);
        });

        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(d => d.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
