using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Contracts;
using Services;
using MAndG.Common.Logging;

var logFilePath = "Logs/weather-log.txt";
Directory.CreateDirectory(Path.GetDirectoryName(logFilePath)!);


using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.AddFileLogger(logFilePath);
    })
    .ConfigureAppConfiguration((context, config) =>
    {
        var env = context.HostingEnvironment;

        config.AddJsonFile(@"Config\appsettings.json", optional: false, reloadOnChange: true)
              .AddJsonFile(@$"Config\\appsettings.{env.EnvironmentName}.json", optional: true)
              .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        services.AddScoped<ICityProvider, CityFileProvider>();
        services.AddScoped<IWeatherService, OpenWeatherService>();
        services.AddScoped<IWeatherStorage, FileWeatherStorage>();

        services.AddScoped<IWeatherSyncProcessor, WeatherSyncProcessor>();
    })
    .Build();

var app = host.Services.GetService<IWeatherSyncProcessor>();
await app!.ProcessDailyWeather();
await host.RunAsync();