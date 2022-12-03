using ConsoleGenericHost;

IHostBuilder builder = Host.CreateDefaultBuilder();

if (args.Contains("--task-a"))
{
    builder.ConfigureServices(services => services.AddConsoleHostedService<TaskA>());
}
else if (args.Contains("--task-b"))
{
    builder.ConfigureServices(services => services.AddConsoleHostedService<TaskB>());
}
else
{
    builder.ConfigureServices(services => services.AddConsoleHostedService<NoOpTask>());
}

builder.Build().Run();
