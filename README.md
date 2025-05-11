
# Weather Data Collection Service

This is a .NET Core console application that collects daily weather data for a list of cities using the OpenWeather API and stores the data in JSON format.

# Features

- Reads city IDs and names from a file (`city-list.txt`)
- Fetches current weather data from OpenWeatherMap
- Stores the weather data per city per day in structured JSON files
- Output directory structure: `Output/YYYY/MM/DD/{CityId}.json`
- Extensible, testable design following SOLID principles
- Includes unit tests with MSTest and Moq
- Configurable via `appsettings.json`

1. REQUIREMENTS:
----------------
- .NET SDK 8.0
- An OpenWeatherMap API key
- Visual Studio

2. CONFIGURATION

Edit the file: appsettings.json

Example:

{
  "OutputFolder": "D:\\Output",
  "OpenWeatherApiKey": "your_api_key_here"
}


3. RUN:
---------------------
Open a terminal or command prompt and run:

    dotnet restore
    dotnet build
    dotnet run --project WeatherSyncJob

4. OUTPUT STRUCTURE

Weather data is stored in:

    Output/YYYY/MM/DD/{CityId}.json

Each file contains raw weather JSON data from the API.