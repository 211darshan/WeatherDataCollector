using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Contracts;
using Services;

//Console.WriteLine("Hello, World!");
//var host = Host.CreateDefaultBuilder(args)
//            .ConfigureServices((context, services) =>
//            {
//                // Bind configuration
//                //services.Configure<AppSettings>(context.Configuration.GetSection("AppSettings"));

//                //// Register application services
//                //services.AddScoped<ICityFileReader, CityFileReader>();
//                //services.AddScoped<IWeatherService, OpenWeatherService>();
//                //services.AddScoped<IFileWriter, FileWriter>();

//                //// Register background service (can be custom too)
//                services.AddTransient<IOpenWeatherService, OpenWeatherService>();
//                services.AddHostedService<WeatherSyncProcessor>();

//            })
//            .Build();

////HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

////builder.Services.AddTransient<IOpenWeatherService, OpenWeatherService>();

////using IHost host = builder.Build();

//await host.RunAsync();


using IHost host = Host.CreateDefaultBuilder(args)
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

// code

var app = host.Services.GetService<IWeatherSyncProcessor>();
//await app!.GetWeatherByCityIdAsync(123);
await app!.ProcessDailyWeather();

await host.RunAsync();