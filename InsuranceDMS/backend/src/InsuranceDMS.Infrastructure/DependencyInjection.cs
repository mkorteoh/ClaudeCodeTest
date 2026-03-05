using InsuranceDMS.Application.Interfaces;
using InsuranceDMS.Infrastructure.Data;
using InsuranceDMS.Infrastructure.Interceptors;
using InsuranceDMS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InsuranceDMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<AuditInterceptor>();
        services.AddSingleton<SoftDeleteInterceptor>();

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
            options.AddInterceptors(
                sp.GetRequiredService<AuditInterceptor>(),
                sp.GetRequiredService<SoftDeleteInterceptor>());
        });

        services.AddScoped<IAgencyRepository, AgencyRepository>();
        services.AddScoped<IPersonnelRepository, PersonnelRepository>();
        services.AddScoped<IProducerRepository, ProducerRepository>();
        services.AddScoped<ILicenseRepository, LicenseRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IMergerRepository, MergerRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
