using InsuranceDMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceDMS.Infrastructure.Data.Configurations;

public class AgencyConfiguration : IEntityTypeConfiguration<Agency>
{
    public void Configure(EntityTypeBuilder<Agency> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("AgencyId");
        builder.Property(x => x.AgencyName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.NPN).HasMaxLength(20);
        builder.HasIndex(x => x.NPN).IsUnique();
        builder.Property(x => x.TaxId).HasMaxLength(20);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.Ignore(x => x.RowVersion);

        // Self-referencing hierarchy
        builder.HasOne(x => x.ParentAgency)
            .WithMany(x => x.ChildAgencies)
            .HasForeignKey(x => x.ParentAgencyId)
            .OnDelete(DeleteBehavior.NoAction);

        // M&A self-ref
        builder.HasOne(x => x.MergedIntoAgency)
            .WithMany()
            .HasForeignKey(x => x.MergedIntoId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
