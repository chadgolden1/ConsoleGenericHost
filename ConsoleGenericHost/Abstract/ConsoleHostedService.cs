namespace ConsoleGenericHost.Abstract;

/// <summary>
/// Base class for implementing a console-based <see cref="IHostedService"/>.
/// </summary>
public abstract class ConsoleHostedService : IHostedService
{
    private ILogger? _logger;
    private IHostApplicationLifetime? _appLifetime;

    private Task? _applicationTask;
    private int? _exitCode;

    /// <summary>
    /// Sets the (required) logger instance to use with this instance.
    /// </summary>
    /// <param name="logger">The logger instance to use with this instance.</param>
    /// <returns>This instance</returns>
    public ConsoleHostedService WithLogger(ILogger logger)
    {
        _logger = logger;
        return this;
    }

    /// <summary>
    /// Sets the (required) host application lifetime instance to use with this instance.
    /// </summary>
    /// <param name="appLifetime">The application lifetime instance to use with this instance.</param>
    /// <returns>This instance</returns>
    public ConsoleHostedService WithHostApplicationLifetime(IHostApplicationLifetime appLifetime)
    {
        _appLifetime = appLifetime;
        return this;
    }

    /// <summary>
    /// This method is called when the <see cref="IHostedService"/> starts. The implementation should return a task that
    /// represents the lifetime of the console operation being performed.
    /// </summary>
    /// <param name="cancellationToken">The stopping token</param>
    /// <returns>A <see cref="Task"/> that represents the console operation.</returns>
    protected abstract Task ExecuteAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous Start operation.</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(_logger);
        ArgumentNullException.ThrowIfNull(_appLifetime);

        CancellationTokenSource? _cancellationTokenSource = null;

        _appLifetime.ApplicationStarted.Register(() =>
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _applicationTask = Task.Run(async () =>
            {
                try
                {
                    await ExecuteAsync(cancellationToken);
                    _exitCode = 0;
                }
                catch (TaskCanceledException)
                {
                    // This means the application is shutting down, so just swallow this exception
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception starting service");
                    _exitCode = 1;
                }
                finally
                {
                    // Stop the application once the work is done
                    _appLifetime.StopApplication();
                }
            });
        });

        _appLifetime.ApplicationStopping.Register(() =>
        {
            _logger.LogDebug("Application is stopping");
            _cancellationTokenSource?.Cancel();
        });

        return Task.CompletedTask;
    }

    /// <summary>
    /// Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous Stop operation.</returns>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        // Wait for the application logic to fully complete any cleanup tasks.
        // Note that this relies on the cancellation token to be properly used in the application.
        if (_applicationTask != null)
        {
            await _applicationTask;
        }

        _logger?.LogDebug("Exiting with return code: {_exitCode}", _exitCode);

        // Exit code may be null if the user cancelled via Ctrl+C/SIGTERM
        Environment.ExitCode = _exitCode.GetValueOrDefault(-1);
    }
}
