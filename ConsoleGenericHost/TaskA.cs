using ConsoleGenericHost.Abstract;

namespace ConsoleGenericHost;

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
