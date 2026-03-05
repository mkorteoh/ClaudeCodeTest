using InsuranceDMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceDMS.Infrastructure.Data.Configurations;

public class ProducerConfiguration : IEntityTypeConfiguration<Producer>
{
    public void Configure(EntityTypeBuilder<Producer> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ProducerId").UseIdentityColumn();
        builder.Property(x => x.NPN).HasMaxLength(20);
        builder.HasIndex(x => x.NPN).IsUnique().HasFilter("[NPN] IS NOT NULL AND [IsDeleted] = 0");
        builder.Property(x => x.ResidentState).HasColumnType("CHAR(2)");
        builder.Property(x => x.SSNLast4).HasColumnType("CHAR(4)");
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);

        builder.HasOne(x => x.ResidentStateNav)
            .WithMany()
            .HasForeignKey(x => x.ResidentState)
            .HasPrincipalKey(s => s.StateCode)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
