using Contracts;
using Enities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CityFileProvider : ICityProvider
    {
        public Task<List<City>> GetCitiesAsync()
        {
            var lines = File.ReadAllLines("city-list.txt");
            var cities = lines.Select(line => {
                var parts = line.Split('=');
                return new City { Id = int.Parse(parts[0]), Name = parts[1] };
            }).ToList();

            return Task.FromResult(cities);
        }
    }
}
