# ConsoleGenericHost
Implements a base for using console-based `IHostedService`'s with the .NET Generic Host.

Implementation based on [dfederm/GenericHostConsoleApp](https://github.com/dfederm/GenericHostConsoleApp) ([blog post](https://dfederm.com/building-a-console-app-with-.net-generic-host/))

## Usage
Usage is similar to [`BackgroundService`](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-7.0&tabs=visual-studio#backgroundservice-base-class)

Create a type that derives from `ConsoleHostedService`, and implement the `ExecuteAsync` method.

```csharp
public class TaskA : ConsoleHostedService
{
    private readonly ILogger<TaskA> _logger;

    public TaskA(ILogger<TaskA> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Running Task A...");
        await Task.Delay(1000, cancellationToken);
    }
}
```

When configuring your services, use the `AddConsoleHostedService` extension.

```csharp
.ConfigureServices(services => services.AddConsoleHostedService<TaskA>());
```

The application will exit when `ExecuteAsync` completes its work.