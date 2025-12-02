#nullable enable

using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace WebApp.Server.Infrastructure;

public static class ValidationExtensions
{
    /// <summary>
    /// Validates a request using the provided validator and returns a ValidationProblemDetails if validation fails.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request to validate.</typeparam>
    /// <param name="validator">The FluentValidation validator for the request.</param>
    /// <param name="request">The request object to validate.</param>
    /// <returns>
    /// A ValidationProblemDetails object if validation fails, or null if validation succeeds.
    /// </returns>
    public static ValidationProblemDetails? ValidateRequest<TRequest>(this IValidator<TRequest> validator, TRequest request)
    {
        var validationResult = validator.Validate(request);
        
        if (validationResult.IsValid)
        {
            return null;
        }

        return GetValidationProblemDetails(validationResult);
    }

    /// <summary>
    /// Validates a request using the provided validator and returns a ValidationProblemDetails if validation fails.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request to validate.</typeparam>
    /// <param name="validator">The FluentValidation validator for the request.</param>
    /// <param name="request">The request object to validate.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>
    /// A ValidationProblemDetails object if validation fails, or null if validation succeeds.
    /// </returns>
    public static async Task<ValidationProblemDetails?> ValidateRequestAsync<TRequest>(this IValidator<TRequest> validator, TRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        
        if (validationResult.IsValid)
        {
            return null;
        }

        return GetValidationProblemDetails(validationResult);
    }

    /// <summary>
    /// Creates a <see cref="ValidationProblemDetails"/> instance representing validation errors.
    /// </summary>
    /// <param name="validationResult">The result of the validation process containing error details.</param>
    /// <returns>A <see cref="ValidationProblemDetails"/> object with a title, status code, detail message, and a dictionary of validation errors.</returns>
    private static ValidationProblemDetails GetValidationProblemDetails(ValidationResult validationResult) => new()
    {
        Title = "Validation Error",
        Status = StatusCodes.Status400BadRequest,
        Detail = "One or more validation errors occurred.",
        Errors = validationResult.ToDictionary()
    };
}
