using FluentValidation;
using MediatR;
using SimpleCut.Common.Dtos;
using SimpleCut.Infrastructure.Cqrs;

namespace SimpleCut.Infrastructure.BehaviourPipelines
{
    public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, OperationResult>
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
            var operationErrors = new HashSet<OperationError>();

            foreach(var validator in _validators)
            {
                var validationResult = validator.Validate(request);

                validationResult.Errors.ForEach(x =>
                    operationErrors.Add(new OperationError(x.PropertyName, x.ErrorMessage)));
            }

            if (operationErrors.Any())
            {
                var errorResult = new OperationResult(operationErrors);

                return errorResult;
            }

            return await next();
        }
    }
}
