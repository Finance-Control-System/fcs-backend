using Application.Abstractions.Cryptography;
using Application.Abstractions.Providers;
using Domain.Configuration;
using Infrastructure.Cryptography;
using Infrastructure.DataAccess;
using Infrastructure.Providers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;


namespace Infrastructure;

public static class DependecyInjection
{
    public static IServiceCollection AddInfranstructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<FinanceCSContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("FinanceCSLocalDbConnectionString"),
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null
                        );
                    sqlOptions.CommandTimeout(180);
                }), ServiceLifetime.Scoped);

        var serviceProvider = services.BuildServiceProvider();
        ConfigureAuth(services, serviceProvider);

        services.AddHttpClient();
        services.AddTransient<IPasswordHasher, PasswordHasher>();
        services.AddTransient<IPasswordHashChecker, PasswordHasher>();
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddHttpContextAccessor();

        services.AddRepositories();

        return services;
    }

    private static void ConfigureAuth(IServiceCollection services, ServiceProvider serviceProvider)
    {
        var valueSecret = serviceProvider
                .GetRequiredService<IOptions<AppSettings>>().Value.Secret;

        services.AddAuthorization();

        services
                .AddAuthentication(x => {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x => {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.ASCII.GetBytes(valueSecret)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        var types = Assembly.GetExecutingAssembly().GetTypes();

        // Find all classes in the implementation namespace
        var implementationTypes = types
            .Where(type => type.IsClass && !type.IsAbstract && type.Namespace == "Infrastructure.Repositories");

        foreach (var implementationType in implementationTypes)
        {
            // Find the corresponding interface in the interface namespace
            var interfaceType = implementationType.GetInterfaces()
                .FirstOrDefault(i => i.Namespace == "Application.Abstractions.Repositories" && $"I{implementationType.Name}" == i.Name);

            if (interfaceType != null)
                services.AddScoped(interfaceType, implementationType);
        }

        return services;
    }
}
