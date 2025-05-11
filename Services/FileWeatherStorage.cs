using Contracts;
using Enities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Services
{
    public class FileWeatherStorage: IWeatherStorage
    {
        private readonly IConfiguration _config;
        public FileWeatherStorage(IConfiguration config)
        {
            _config = config;
        }
        public async Task SaveAsync(Weather weather)
        {
            string folderPath = _config["OutputFolder"] != null ? _config["OutputFolder"] : @"C:\output";
            var dateFolder = Path.Combine(folderPath, DateTime.UtcNow.ToString("yyyy"), DateTime.UtcNow.ToString("MM"), DateTime.UtcNow.ToString("dd"));
            if (!Directory.Exists(dateFolder))
            {
                Directory.CreateDirectory(dateFolder);
            }
            var filePath = Path.Combine(dateFolder, $"{weather.City.Id}.json");
            await File.WriteAllTextAsync(filePath, weather.Data);
        }
    }
}
