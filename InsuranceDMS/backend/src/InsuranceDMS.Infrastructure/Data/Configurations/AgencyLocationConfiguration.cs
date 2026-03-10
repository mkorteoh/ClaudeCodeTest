using InsuranceDMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceDMS.Infrastructure.Data.Configurations;

public class AgencyLocationConfiguration : IEntityTypeConfiguration<AgencyLocation>
{
    public void Configure(EntityTypeBuilder<AgencyLocation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("AgencyLocationId");
        builder.Property(x => x.LocationName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Phone).HasMaxLength(20);
        builder.Property(x => x.Email).HasMaxLength(200);
        builder.Property(x => x.Website).HasMaxLength(300);
        builder.Property(x => x.AddressLine1).HasMaxLength(200);
        builder.Property(x => x.AddressLine2).HasMaxLength(200);
        builder.Property(x => x.City).HasMaxLength(100);
        builder.Property(x => x.StateCode).HasMaxLength(2);
        builder.Property(x => x.ZipCode).HasMaxLength(10);
        builder.Property(x => x.County).HasMaxLength(100);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.Ignore(x => x.RowVersion);

        builder.HasIndex(x => new { x.AgencyId, x.IsCorporateOffice });

        builder.HasOne(x => x.Agency)
            .WithMany(x => x.Locations)
            .HasForeignKey(x => x.AgencyId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.StateNav)
            .WithMany()
            .HasForeignKey(x => x.StateCode)
            .HasPrincipalKey(s => s.StateCode)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Agency>()
            .WithMany()
            .HasForeignKey(x => x.OriginalAgencyId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

public class PersonnelLocationConfiguration : IEntityTypeConfiguration<PersonnelLocation>
{
    public void Configure(EntityTypeBuilder<PersonnelLocation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("PersonnelLocationId");
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);

        builder.HasIndex(x => new { x.PersonnelId, x.AgencyLocationId }).IsUnique();

        builder.HasOne(x => x.Personnel)
            .WithMany()
            .HasForeignKey(x => x.PersonnelId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.AgencyLocation)
            .WithMany(x => x.PersonnelLocations)
            .HasForeignKey(x => x.AgencyLocationId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
