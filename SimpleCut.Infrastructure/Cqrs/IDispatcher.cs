using MediatR;
using SimpleCut.Common.Dtos;

namespace SimpleCut.Infrastructure.Cqrs
{
    public interface IDispatcher
    {
        public Task<OperationResult<TResult>> SendAsync<TResult>(IQuery<TResult> query, CancellationToken token = default);

        public Task<OperationResult> SendAsync(ICommand command, CancellationToken token = default);
    }
}
