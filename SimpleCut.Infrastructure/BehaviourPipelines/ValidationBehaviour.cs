using FluentValidation;
using MediatR;
using SimpleCut.Common.Dtos;
using SimpleCut.Infrastructure.Cqrs;

namespace SimpleCut.Infrastructure.BehaviourPipelines
{
    public sealed class ValidationBehavior<TRequest> : IPipelineBehavior<TRequest, OperationResult>
        where TRequest : class, ICommand
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;
        public async Task<OperationResult> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<OperationResult> next)
        {
            if (!_validators.Any())
            {
                return await next();
            }
            var context = new ValidationContext<TRequest>(request);
            var operationErrors = _validators
                .Select(x => x.Validate(context))
                .SelectMany(x => x.Errors)
                .Where(x => x != null)
                .Select(x => new OperationError(x.ErrorMessage, x.PropertyName));

            if (operationErrors.Any())
            {
                var errorResult = new OperationResult(operationErrors);

                return errorResult;
            }

            return await next();
        }
    }
}
