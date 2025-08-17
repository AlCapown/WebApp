#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Common.Infrastructure;

public static class QueryHelpers
{
    public static string AddQueryString(string uri, Dictionary<string, string> queryPrams)
    {
        if (queryPrams is null || queryPrams.Count == 0)
        {
            return uri;
        }

        string queryString = string.Join("&", queryPrams
            .Where(kvp => kvp.Key is not null)
            .Where(kvp => kvp.Value is not null)
            .Select(kvp => string.Concat(Uri.EscapeDataString(kvp.Key), "=", Uri.EscapeDataString(kvp.Value))));

        if (queryString == string.Empty)
        {
            return uri;
        }

        return string.Concat(uri.TrimEnd('/'), "?", queryString);
    }
}
