using MediatR;
using SimpleCut.Common.Dtos;
using SimpleCut.Common.Exceptions;

namespace SimpleCut.Infrastructure.Cqrs
{
    public class Dispatcher : IDispatcher
    {
        private readonly IMediator _mediator;

        public Dispatcher(IMediator mediator)
        {
            this._mediator = mediator;
        }

        public async Task<OperationResult<TResult>> Send<TResult>(IRequest<OperationResult<TResult>> query, CancellationToken token = default)
        {
            var result = await _mediator.Send<OperationResult<TResult>>(query, token);

            // TODO.DA i do not like it. I will try find another apporach
            if (!result.Success)
            {
                throw new SimpleCutValidationExpection(result.JoinErrors());
            }

            return result;
        }
    }
}
