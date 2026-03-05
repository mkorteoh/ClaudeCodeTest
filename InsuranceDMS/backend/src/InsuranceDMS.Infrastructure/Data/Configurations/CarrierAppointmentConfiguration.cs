using InsuranceDMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceDMS.Infrastructure.Data.Configurations;

public class CarrierAppointmentConfiguration : IEntityTypeConfiguration<CarrierAppointment>
{
    public void Configure(EntityTypeBuilder<CarrierAppointment> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("AppointmentId");
        builder.Property(x => x.StateCode).HasMaxLength(2).IsRequired();
        builder.Property(x => x.AppointmentStatus).HasMaxLength(50);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);

        builder.HasIndex(x => new { x.ProducerId, x.CarrierId, x.StateCode }).IsUnique();

        builder.HasOne(x => x.Producer)
            .WithMany(x => x.Appointments)
            .HasForeignKey(x => x.ProducerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Carrier)
            .WithMany(x => x.Appointments)
            .HasForeignKey(x => x.CarrierId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.State)
            .WithMany()
            .HasForeignKey(x => x.StateCode)
            .HasPrincipalKey(s => s.StateCode)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
