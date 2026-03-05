using InsuranceDMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceDMS.Infrastructure.Data.Configurations;

public class LicenseTypeConfiguration : IEntityTypeConfiguration<LicenseType>
{
    public void Configure(EntityTypeBuilder<LicenseType> builder)
    {
        builder.HasKey(x => x.LicenseTypeId);
        builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(200).IsRequired();
    }
}
