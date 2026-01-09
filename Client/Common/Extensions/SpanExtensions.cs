#nullable enable

using System;
using System.Collections.Generic;

namespace WebApp.Client.Common.Extensions;

public static class SpanExtensions
{
    public static KeyValuePair<TKey, TSource>[] ToKeyValuePairs<TKey, TSource>(this ReadOnlySpan<TSource> source, Func<TSource, TKey> keySelector) 
        where TKey : notnull

    {
        ArgumentNullException.ThrowIfNull(keySelector);

        if (source.Length == 0)
        {
            return [];
        }

        var result = new KeyValuePair<TKey, TSource>[source.Length];

        for (int i = 0; i < source.Length; i++)
        {
            result[i] = new KeyValuePair<TKey, TSource>(keySelector(source[i]), source[i]);
        }

        return result;
    }
}
