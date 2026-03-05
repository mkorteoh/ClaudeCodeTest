using InsuranceDMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceDMS.Infrastructure.Data.Configurations;

public class StateConfiguration : IEntityTypeConfiguration<State>
{
    public void Configure(EntityTypeBuilder<State> builder)
    {
        builder.HasKey(x => x.StateCode);
        builder.Property(x => x.StateCode).HasMaxLength(2).IsRequired();
        builder.Property(x => x.StateName).HasMaxLength(100).IsRequired();
    }
}
