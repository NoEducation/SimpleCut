using MediatR;

namespace SimpleCut.Logic.EmptyFeature.Queries
{
    public class GetWeatherForecastQuery : IRequest<GetWeatherForecastQueryResponse>
    {
    }
}
