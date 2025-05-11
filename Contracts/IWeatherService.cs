using Enities;

namespace Contracts
{
    public interface IWeatherService
    {
        Task<Weather> GetWeatherByCity(City city);
    }
}
