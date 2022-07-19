using Dapper.Contrib.Extensions;
using MediatR;
using SimpleCut.Common.Dtos;
using SimpleCut.Domain.EmptyFeature;
using SimpleCut.Domain.Users;
using SimpleCut.Infrastructure.Context;

namespace SimpleCut.Logic.EmptyFeature.Queries
{
    // It's just for testing archtecture purpose
    public class GetWeatherForecastQueryHandler : IRequestHandler<GetWeatherForecastQuery, OperationResult<GetWeatherForecastQueryResponse>>
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IDbContext _context;

        public GetWeatherForecastQueryHandler(IDbContext context)
        {
            _context = context;
        }

        public async Task<OperationResult<GetWeatherForecastQueryResponse>> Handle(GetWeatherForecastQuery request, CancellationToken cancellationToken)
        {
            var weatherForecasts =  Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            }).ToArray();

            var cars = await _context.Connection.GetAllAsync<Role>();


 //           await _context.ExecuteInTransation(x => x.Execute(@"
 //INSERT INTO public.films(
	//code, title, did, dateprod, kind)
	//VALUES ('test1', 'test1', 1, now(), 'test');"),
 //   x => x.Execute(@"INSERT INTO public.films(
	//code, title, did, dateprod, kind)
	//VALUES ('test2', 'test1', 1, now(), 'test');"));


            return await Task.FromResult(new OperationResult<GetWeatherForecastQueryResponse>()
            {
                Result = new GetWeatherForecastQueryResponse()
                {
                    WeatherForecasts = weatherForecasts
                }
            });
        }
    }
}
