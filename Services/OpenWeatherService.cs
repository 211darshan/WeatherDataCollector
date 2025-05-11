using Contracts;
using Enities;
using Microsoft.Extensions.Configuration;

namespace Services
{
    public class OpenWeatherService: IWeatherService
    {
        private readonly IConfiguration _config;
        public OpenWeatherService(IConfiguration config)
        {
            _config = config;
        }
        public async Task<Weather> GetWeatherByCity(City city)
        {
            var apiKey = _config["OpenWeatherService:AppId"];
            var baseUrl = _config["OpenWeatherService:BaseUrl"];

            string url = $"{baseUrl}?id={city.Id}&appid={apiKey}";

            using var httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            return new Weather()
            {
                City = city,
                Data = json,
            };
        }
    }
}
