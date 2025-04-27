using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Immutable;
using System.Linq;
using WebApp.Common.Models;

namespace WebApp.Server.Infrastructure.Exceptions;

public static class ApiModelStateValidationHandler
{
    public static IServiceCollection AddCustomErrorHandlerForModelStateValidation(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = actionContext =>
            {
                return new ErrorResult(actionContext.ModelState.MapToApiError());
            };
        });

        return services;
    }

    private static ApiError MapToApiError(this ModelStateDictionary modelState)
    {
        var dictionaryBuilder = ImmutableDictionary.CreateBuilder<string, ImmutableList<string>>();

        foreach (var key in modelState.Keys)
        {
            var listBuilder = ImmutableList.CreateBuilder<string>();
            listBuilder.AddRange(modelState[key].Errors.Select(x => x.ErrorMessage));
            dictionaryBuilder.Add(key, listBuilder.ToImmutableList());
        }

        return new ApiError
        {
            StatusCode = 400,
            Message = "The following request properties are invalid.",
            Errors = dictionaryBuilder.ToImmutableDictionary()
        };
    }
}
