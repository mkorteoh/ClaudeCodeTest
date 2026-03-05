using InsuranceDMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceDMS.Infrastructure.Data.Configurations;

public class MergerConfiguration : IEntityTypeConfiguration<Merger>
{
    public void Configure(EntityTypeBuilder<Merger> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("MergerId").UseIdentityColumn();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(x => x.InitiatedBy).HasMaxLength(100);
        builder.Property(x => x.ExecutedBy).HasMaxLength(100);
        builder.Property(x => x.Notes).HasMaxLength(2000);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);

        builder.HasOne(x => x.SurvivingAgency)
            .WithMany()
            .HasForeignKey(x => x.SurvivingAgencyId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

public class MergerParticipantConfiguration : IEntityTypeConfiguration<MergerParticipant>
{
    public void Configure(EntityTypeBuilder<MergerParticipant> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ParticipantId").UseIdentityColumn();
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);

        builder.HasIndex(x => new { x.MergerId, x.AbsorbedAgencyId }).IsUnique();

        builder.HasOne(x => x.Merger)
            .WithMany(x => x.Participants)
            .HasForeignKey(x => x.MergerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.AbsorbedAgency)
            .WithMany()
            .HasForeignKey(x => x.AbsorbedAgencyId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

public class EntityLineageConfiguration : IEntityTypeConfiguration<EntityLineage>
{
    public void Configure(EntityTypeBuilder<EntityLineage> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("LineageId").UseIdentityColumn();
        builder.Property(x => x.EntityType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Action).HasMaxLength(50);
        builder.Property(x => x.RecordedBy).HasMaxLength(100);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);

        builder.HasOne(x => x.Merger)
            .WithMany(x => x.Lineage)
            .HasForeignKey(x => x.MergerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.SourceAgency)
            .WithMany()
            .HasForeignKey(x => x.SourceAgencyId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.TargetAgency)
            .WithMany()
            .HasForeignKey(x => x.TargetAgencyId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(x => x.AuditLogId);
        builder.Property(x => x.TableName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Action).HasMaxLength(20).IsRequired();
        builder.Property(x => x.ColumnName).HasMaxLength(100);
        builder.Property(x => x.ChangedBy).HasMaxLength(100);
        builder.Property(x => x.CorrelationId).HasMaxLength(100);
    }
}
