using ConsoleGenericHost.Abstract;

namespace ConsoleGenericHost;

public class TaskB : ConsoleHostedService
{
    private readonly ILogger<TaskB> _logger;

    public TaskB(ILogger<TaskB> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Running Task B...");
        await Task.Delay(1000, cancellationToken);
    }
}
