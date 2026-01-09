#nullable enable

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

        var sb = new StringBuilder();

        for (int i = 0; i < queryParameters.Length; i++)
        {
            AppendKeyValuePair(sb, queryParameters[i]);
        }

        return string.Concat(uri.TrimEnd('/'), "?", sb.ToString());
    }

    private static void AppendKeyValuePair(StringBuilder sb, KeyValuePair<string, string?> kvp)
    {
        if (kvp.Key is null || kvp.Value is null)
        {
            return;
        }

        if (sb.Length > 0)
        {
            sb.Append('&');
        }

        sb.Append(Uri.EscapeDataString(kvp.Key));
        sb.Append('=');
        sb.Append(Uri.EscapeDataString(kvp.Value));
    }
}
