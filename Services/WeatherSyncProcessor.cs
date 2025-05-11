using Contracts;
using Enities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class WeatherSyncProcessor: IWeatherSyncProcessor
    {
        private readonly ICityProvider _provider;
        private readonly IWeatherService _service;
        private readonly IWeatherStorage _storage;
        private readonly ILogger<WeatherSyncProcessor> _logger;
        public WeatherSyncProcessor(ICityProvider provider, IWeatherService service, IWeatherStorage storage, ILogger<WeatherSyncProcessor> logger)
        {
            _provider = provider;
            _service = service;
            _storage = storage;
            _logger = logger;
        }
        public async Task ProcessDailyWeather()
        {
            _logger.LogInformation("Started");
            var cities = await _provider.GetCitiesAsync();
            foreach (var city in cities)
            {
                var weather = await _service.GetWeatherByCity(city);
                await _storage.SaveAsync(weather);

            }
        }
    }
}
