using MediatR;
using SimpleCut.Common.Dtos;

namespace SimpleCut.Logic.EmptyFeature.Queries
{
    public class GetWeatherForecastQuery : IRequest<OperationResult<GetWeatherForecastQueryResponse>>
    {
    }
}
