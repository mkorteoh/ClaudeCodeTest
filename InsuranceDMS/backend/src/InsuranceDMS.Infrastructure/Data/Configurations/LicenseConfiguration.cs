using InsuranceDMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceDMS.Infrastructure.Data.Configurations;

public class LicenseConfiguration : IEntityTypeConfiguration<License>
{
    public void Configure(EntityTypeBuilder<License> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("LicenseId");
        builder.Property(x => x.StateCode).HasMaxLength(2).IsRequired();
        builder.Property(x => x.LicenseNumber).HasMaxLength(50).IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);

        builder.HasOne(x => x.Producer)
            .WithMany(x => x.Licenses)
            .HasForeignKey(x => x.ProducerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.State)
            .WithMany()
            .HasForeignKey(x => x.StateCode)
            .HasPrincipalKey(s => s.StateCode)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.LicenseType)
            .WithMany()
            .HasForeignKey(x => x.LicenseTypeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
