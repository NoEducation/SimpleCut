using MediatR;
using SimpleCut.Common.Dtos;

namespace SimpleCut.Infrastructure.Cqrs
{
    public interface ICommand : IRequest<OperationResult>
    {}
}
