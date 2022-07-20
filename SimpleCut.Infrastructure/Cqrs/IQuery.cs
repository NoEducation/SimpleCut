using MediatR;
using SimpleCut.Common.Dtos;

namespace SimpleCut.Infrastructure.Cqrs
{
    public interface IQuery<TResponse> : IRequest<OperationResult<TResponse>>
    { }
}
