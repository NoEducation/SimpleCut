using MediatR;
using Microsoft.AspNetCore.Mvc;
using SimpleCut.Logic.EmptyFeature.Queries;

namespace SimpleCut.Api.Controllers
{
    public class WeatherForecastController : BaseController
    {
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IMediator mediator) : base(mediator)
        {}

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<GetWeatherForecastQueryResponse> Get()
        {
            var response = await Mediator.Send(new GetWeatherForecastQuery());

            return response;
        }
    }
}