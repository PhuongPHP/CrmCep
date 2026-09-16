using CrmCep.Application.Interfaces;
using CrmCep.Infrastructure.Data;
using CrmCep.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CrmCep.Infrastructure;

/// <summary>
/// Infrastructure dependency injection extensions configuring SQL Server and application business services.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var sqlServerConn = configuration.GetConnectionString("DefaultConnection") 
            ?? "Server=localhost;Database=CrmCepDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;";

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(sqlServerConn, b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Register authentication and security services
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICustomerService, CustomerService>();

        return services;
    }
}
