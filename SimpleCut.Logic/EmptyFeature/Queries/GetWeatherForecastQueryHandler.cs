using MediatR;
using SimpleCut.Domain.EmptyFeature;
using SimpleCut.Infrastructure.Context;
using Dapper;
using Dapper.Contrib.Extensions;

namespace SimpleCut.Logic.EmptyFeature.Queries
{
    // It's just for testing archtecture purpose
    public class GetWeatherForecastQueryHandler : IRequestHandler<GetWeatherForecastQuery, GetWeatherForecastQueryResponse>
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

        public async Task<GetWeatherForecastQueryResponse> Handle(GetWeatherForecastQuery request, CancellationToken cancellationToken)
        {
            var weatherForecasts =  Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            }).ToArray();

            var cars = await _context.Connection.GetAllAsync<Film>();

            _context.BeginTransaction();

            _context.Connection.Execute(@"INSERT INTO public.films(
	code, title, did, dateprod, kind)
	VALUES ('test3', 'test1', 1, now(), 'test');");
            _context.Connection.Execute(@"INSERT INTO public.films(
	code, title, did, dateprod, kind)
	VALUES ('test4', 'test1', 1, now(), 'test');");

            _context.CommitTransaction();


 //           await _context.ExecuteInTransation(x => x.Execute(@"
 //INSERT INTO public.films(
	//code, title, did, dateprod, kind)
	//VALUES ('test1', 'test1', 1, now(), 'test');"),
 //   x => x.Execute(@"INSERT INTO public.films(
	//code, title, did, dateprod, kind)
	//VALUES ('test2', 'test1', 1, now(), 'test');"));


            return await Task.FromResult(new GetWeatherForecastQueryResponse()
            {
                WeatherForecasts = weatherForecasts
            });
        }
    }
}
