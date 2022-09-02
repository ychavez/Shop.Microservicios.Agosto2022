using FluentValidation;
using MediatR;

namespace Ordering.Application.Behaviours
{
    public class ValidationBehaviour<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            this.validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {

            // validamos que existan validaciones
            if (validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                //ejecutamos todas las validaciones
                var validationResult =
                    await Task.WhenAll(validators
                    .Select(v => v.ValidateAsync(context, cancellationToken)));

                var failures = validationResult
                                .SelectMany(r => r.Errors)
                                .Where(f => f is not null)
                                .ToList();

                if (failures.Any())
                    throw new ValidationException(failures);
            }

            return await next();

        }
    }
}
