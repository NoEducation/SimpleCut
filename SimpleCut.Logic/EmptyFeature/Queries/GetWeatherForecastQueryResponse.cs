using SimpleCut.Domain.EmptyFeature;

namespace SimpleCut.Logic.EmptyFeature.Queries
{
    public class GetWeatherForecastQueryResponse
    {
        public IEnumerable<WeatherForecast>? WeatherForecasts { get; set; }
    }
}
