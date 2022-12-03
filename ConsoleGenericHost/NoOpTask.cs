using ConsoleGenericHost.Abstract;

namespace ConsoleGenericHost;

public class NoOpTask : ConsoleHostedService
{
    private ILogger<NoOpTask> _logger;

    public NoOpTask(ILogger<NoOpTask> logger)
    {
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogWarning("No task was registered. The application will exit.");
        return Task.CompletedTask;
    }
}