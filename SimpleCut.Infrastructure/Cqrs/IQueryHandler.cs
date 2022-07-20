using MediatR;
using SimpleCut.Common.Dtos;

namespace SimpleCut.Infrastructure.Cqrs
{
    public interface IQueryHandler<TQuery, TResult> : IRequestHandler<TQuery, OperationResult<TResult>>
        where TQuery : IRequest<OperationResult<TResult>>
    {
    }
}
