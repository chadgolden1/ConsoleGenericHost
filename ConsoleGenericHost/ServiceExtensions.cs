using ConsoleGenericHost.Abstract;

namespace ConsoleGenericHost;

public static class ServiceExtensions
{
    /// <summary>
    /// Adds the console hosted service and required dependencies to the services.
    /// </summary>
    /// <typeparam name="T">The service type that derives from <see cref="ConsoleHostedService"/></typeparam>
    /// <param name="services">The application services</param>
    /// <returns>The application services</returns>
    public static IServiceCollection AddConsoleHostedService<T>(this IServiceCollection services) where T : ConsoleHostedService => 
        services.AddSingleton<IHostedService, T>(sp => 
            (T)ActivatorUtilities.CreateInstance<T>(sp)
                .WithLogger(sp.GetRequiredService<ILogger<T>>())
                .WithHostApplicationLifetime(sp.GetRequiredService<IHostApplicationLifetime>()));
}