using Microsoft.Extensions.DependencyInjection;
using Presentation.Filters;

namespace Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllersWithViews(options =>
        {
            options.Filters.Add(typeof(ValidateModelStateAttribute));
            options.Filters.Add(typeof(HttpResponseExceptionFilter));
        });

        return services;
    }
}
