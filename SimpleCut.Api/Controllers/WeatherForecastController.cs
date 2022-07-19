using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleCut.Infrastructure.Cqrs;
using SimpleCut.Logic.EmptyFeature.Queries;

namespace SimpleCut.Api.Controllers
{
    public class WeatherForecastController : BaseController
    {
        public WeatherForecastController(IDispatcher dispatcher) : base(dispatcher)
        {}

        [AllowAnonymous]
        [HttpGet("GetWeatherForecast")]
        public async Task<ActionResult<GetWeatherForecastQueryResponse>> Get()
        {
            var response = await Dispatcher.Send<GetWeatherForecastQueryResponse>(new GetWeatherForecastQuery());

            return response.Result;
        }

        [HttpGet("TryGetWeatherForecast")]
        public async Task<ActionResult<GetWeatherForecastQueryResponse>> TryGet()
        {
            var response = await Dispatcher.Send<GetWeatherForecastQueryResponse>(new GetWeatherForecastQuery());

            return response.Result;
        }
    }
}