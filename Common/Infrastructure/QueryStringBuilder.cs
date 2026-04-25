using System;
using System.Collections.Generic;
using System.Text;

namespace WebApp.Common.Infrastructure;

public static class QueryHelpers
{
    public static string AddQueryString(string uri, ReadOnlySpan<KeyValuePair<string, string?>> queryParameters)
    {
        if (queryParameters.Length == 0)
        {
            return uri;
        }

        bool hasQuery = false;
        var sb = new StringBuilder(uri.TrimEnd('/'));

        foreach (var kvp in queryParameters)
        {
            if (kvp.Key is null || kvp.Value is null)
            {
                continue;
            }

            if (hasQuery)
            {
                sb.Append('&');
            }
            else
            {
                sb.Append('?');
                hasQuery = true;
            }

            sb.Append(Uri.EscapeDataString(kvp.Key));
            sb.Append('=');
            sb.Append(Uri.EscapeDataString(kvp.Value));
        }

        return hasQuery ? sb.ToString() : uri;
    }
}
