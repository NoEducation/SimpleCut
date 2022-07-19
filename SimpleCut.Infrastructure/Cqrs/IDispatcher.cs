using MediatR;
using SimpleCut.Common.Dtos;

namespace SimpleCut.Infrastructure.Cqrs
{
    public interface IDispatcher
    {
        public Task<OperationResult<TResult>> Send<TResult>(IRequest<OperationResult<TResult>> query, CancellationToken token = default);
    }
}
