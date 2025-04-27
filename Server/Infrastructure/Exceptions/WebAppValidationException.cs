using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace WebApp.Server.Infrastructure.Exceptions;

public sealed class WebAppValidationException : Exception
{
    private readonly ConcurrentDictionary<string, List<string>> _errors;

    public WebAppValidationException()
    {
        _errors = [];
    }

    public WebAppValidationException(string property, string error) : this()
    {
        Add(property, error);
    }

    public void ThrowIfErrors()
    {
        if (!_errors.IsEmpty)
        {
            throw this;
        }
    }

    public void Add(string property, string error)
    {
        _errors.AddOrUpdate(
            key: property,
            addValueFactory: _ => [ error ],
            updateValueFactory: (_, items) => 
            { 
                items.Add(error); 
                return items; 
            }
        );
    }

    public ImmutableDictionary<string, ImmutableList<string>> GetErrors()
    {
        var builder = ImmutableDictionary.CreateBuilder<string, ImmutableList<string>>();

        foreach (var key in _errors.Keys)
        {
            builder.Add(key, [.. _errors[key]]);
        }

        return builder.ToImmutable();
    }
}
