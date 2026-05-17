using System;
using System.Collections.Generic;

namespace WebApp.Client.Common.Extensions;

internal static class SpanExtensions
{
    public static KeyValuePair<TKey, TSource>[] ToKeyValuePairs<TKey, TSource>(this ReadOnlySpan<TSource> source, Func<TSource, TKey> keySelector) 
        where TKey : notnull
    {
        if (source.Length == 0)
        {
            return [];
        }

        var result = new KeyValuePair<TKey, TSource>[source.Length];

        for (int i = 0; i < source.Length; i++)
        {
            var t = source[i];
            result[i] = new KeyValuePair<TKey, TSource>(keySelector(t), t);
        }

        return result;
    }
}
