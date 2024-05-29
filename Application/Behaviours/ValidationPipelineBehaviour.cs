using FluentValidation.Results;
using FluentValidation;
using MediatR;

namespace Application.Behaviours;

internal sealed class ValidationPipelineBehaviour<IRequest, TResponse> : IPipelineBehavior<IRequest, TResponse>
    where IRequest : class
{
    private readonly IEnumerable<IValidator<IRequest>> _validators;

    public ValidationPipelineBehaviour(IEnumerable<IValidator<IRequest>> validators) => _validators = validators;

    public async Task<TResponse> Handle(IRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<IRequest>(request);

        ValidationFailure[] failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToArray();

        if (failures.Length != 0)
            throw new ValidationException(failures);

        return await next();
    }
}
