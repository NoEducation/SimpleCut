using MediatR;
using SimpleCut.Common.Dtos;

namespace SimpleCut.Infrastructure.Cqrs
{
    public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, OperationResult>
        where TCommand : IRequest<OperationResult>
    {}
}
