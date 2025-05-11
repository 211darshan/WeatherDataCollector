using Enities;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration.Json;
using System.IO;
using Moq;
using Contracts;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;

namespace Tests
{
    [TestClass]
    public class WeatherDataCollectionTest
    {
        private IConfiguration _configuration;
        private string _testoutputFolderPath;

        [TestInitialize]
        public void Setup()
        {

            _configuration = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory()) // looks in output directory
           .AddJsonFile("appsettings.Test.json", optional: false)
           .Build();
            _testoutputFolderPath = _configuration["OutputFolder"];
        }

        [TestMethod]
        public async Task GetWeatherByCity_ReturnsJsonData()
        {
            var service = new OpenWeatherService(_configuration);
            var city = new City { Id = 2643741, Name = "City of London" };
            var result = await service.GetWeatherByCity(city);
            Assert.IsTrue(result.Data.Contains("temp_max"));
        }
        [TestMethod]
        public async Task GetCitiesAsync_ReturnsCorrectCities()
        {
            var provider = new CityFileProvider();
            var result = await provider.GetCitiesAsync();
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public async Task SaveAsync_CreatesDirectoryAndWritesFile()
        {
            var weather = new Weather
            {
                City = new City { Id = 123456, Name = "City of London" },
                Data = "{ \"123456\": 42 }"
            };

            var storage = new FileWeatherStorage(_configuration);
            await storage.SaveAsync(weather);


            string dateFolder = Path.Combine(_testoutputFolderPath,
                DateTime.UtcNow.ToString("yyyy"),
                DateTime.UtcNow.ToString("MM"),
                DateTime.UtcNow.ToString("dd"));

            string expectedFile = Path.Combine(dateFolder, "123456.json");

            Assert.IsTrue(File.Exists(expectedFile), "Weather JSON file was not created.");

            string contents = await File.ReadAllTextAsync(expectedFile);
            Assert.AreEqual(weather.Data, contents);
        }

        [TestMethod]
        public async Task ProcessDailyWeather_VerifyForLoopIterations()
        {
            var mockCityProvider = new Mock<ICityProvider>();
            var mockWeatherService = new Mock<IWeatherService>();
            var mockWeatherStorage = new Mock<IWeatherStorage>();
            var mockLogger = new Mock<ILogger<WeatherSyncProcessor>>();

            var cities = new List<City>
            {
                new City { Id = 2643741, Name = "City of London" },
                new City { Id = 2988507, Name = "Paris" }
            };
            mockCityProvider.Setup(p => p.GetCitiesAsync()).ReturnsAsync(cities);

            var weather = new Weather
            {
                City = new City { Id = 1, Name = "City of London" },
                Data = "{ \"temp_max\": 42 }"
            };
            mockWeatherService.Setup(s => s.GetWeatherByCity(It.IsAny<City>())).ReturnsAsync(weather);
            var processor = new WeatherSyncProcessor(mockCityProvider.Object, mockWeatherService.Object, mockWeatherStorage.Object, mockLogger.Object);

            await processor.ProcessDailyWeather();

            mockCityProvider.Verify(p => p.GetCitiesAsync(), Times.Once);
            mockWeatherService.Verify(s => s.GetWeatherByCity(It.IsAny<City>()), Times.Exactly(2));
            mockWeatherStorage.Verify(s => s.SaveAsync(It.IsAny<Weather>()), Times.Exactly(2));
        }

        [TestMethod]
        public async Task ProcessDailyWeather_HandlesEmptyCityList()
        {
            var mockCityProvider = new Mock<ICityProvider>();
            var mockWeatherService = new Mock<IWeatherService>();
            var mockWeatherStorage = new Mock<IWeatherStorage>();
            var mockLogger = new Mock<ILogger<WeatherSyncProcessor>>();
            mockCityProvider.Setup(p => p.GetCitiesAsync()).ReturnsAsync(new List<City>());
            var processor = new WeatherSyncProcessor(mockCityProvider.Object, mockWeatherService.Object, mockWeatherStorage.Object, mockLogger.Object);

            await processor.ProcessDailyWeather();
            mockCityProvider.Verify(p => p.GetCitiesAsync(), Times.Once);
            mockWeatherService.Verify(s => s.GetWeatherByCity(It.IsAny<City>()), Times.Never);
            mockWeatherStorage.Verify(s => s.SaveAsync(It.IsAny<Weather>()), Times.Never);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_testoutputFolderPath))
            {
                Directory.Delete(_testoutputFolderPath, recursive: true);
            }
        }
    }
}
