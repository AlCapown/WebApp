#nullable enable

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace WebApp.Server.Infrastructure;

public sealed class SecurityHeadersMiddleware : IMiddleware
{
    private readonly IHostEnvironment _environment;
    private readonly string _contentSecurityPolicy;

    public SecurityHeadersMiddleware(IHostEnvironment environment)
    {
        _environment = environment;
        _contentSecurityPolicy = environment.IsDevelopment()
            ? string.Join("; ", new[]
            {
                CspDirectives.DefaultSrc,
                CspDirectives.ScriptSrc,
                CspDirectives.StyleSrc,
                CspDirectives.FontSrc,
                CspDirectives.ConnectSrcDevelopment,
                CspDirectives.ImgSrc,
                CspDirectives.FrameSrc
            })
            : string.Join("; ", new[]
            {
                CspDirectives.DefaultSrc,
                CspDirectives.ScriptSrc,
                CspDirectives.StyleSrc,
                CspDirectives.FontSrc,
                CspDirectives.ConnectSrcProduction,
                CspDirectives.ImgSrc,
                CspDirectives.FrameSrc
            });
    }

    public Task InvokeAsync(HttpContext context, RequestDelegate requestDelegate)
    {
        // Security Headers
        context.Response.Headers[HeaderNames.XFrameOptions] = HeaderValues.XFrameOptionsDeny;
        context.Response.Headers[HeaderNames.XContentTypeOptions] = HeaderValues.XContentTypeOptionsNoSniff;
        context.Response.Headers[HeaderNames.ReferrerPolicy] = HeaderValues.ReferrerPolicyStrictOrigin;
        context.Response.Headers[HeaderNames.ContentSecurityPolicy] = _contentSecurityPolicy;


        if (_environment.IsProduction())
        {
            context.Response.Headers[HeaderNames.StrictTransportSecurity] = HeaderValues.StrictTransportSecurity;
        }

        // Cross-origin headers for WASM multithreading support
        context.Response.Headers[HeaderNames.CrossOriginEmbedderPolicy] = HeaderValues.CrossOriginEmbedderPolicyRequireCorp;
        context.Response.Headers[HeaderNames.CrossOriginOpenerPolicy] = HeaderValues.CrossOriginOpenerPolicySameOrigin;

        return requestDelegate(context);
    }

    private static class HeaderNames
    {
        public const string XFrameOptions = "X-Frame-Options";
        public const string XContentTypeOptions = "X-Content-Type-Options";
        public const string ReferrerPolicy = "Referrer-Policy";
        public const string ContentSecurityPolicy = "Content-Security-Policy";
        public const string CrossOriginEmbedderPolicy = "Cross-Origin-Embedder-Policy";
        public const string CrossOriginOpenerPolicy = "Cross-Origin-Opener-Policy";
        public const string StrictTransportSecurity = "Strict-Transport-Security";
    }

    private static class HeaderValues
    {
        public const string XFrameOptionsDeny = "DENY";
        public const string XContentTypeOptionsNoSniff = "nosniff";
        public const string ReferrerPolicyStrictOrigin = "strict-origin-when-cross-origin";
        public const string CrossOriginEmbedderPolicyRequireCorp = "require-corp";
        public const string CrossOriginOpenerPolicySameOrigin = "same-origin";
        public const string StrictTransportSecurity = "max-age=63072000; includeSubDomains; preload";
    }

    private static class CspDirectives
    {
        public const string DefaultSrc = "default-src 'self'";
        public const string ScriptSrc = "script-src 'self' 'wasm-unsafe-eval'";
        public const string StyleSrc = "style-src 'self' 'unsafe-inline' fonts.googleapis.com";
        public const string FontSrc = "font-src 'self' fonts.gstatic.com";
        public const string ConnectSrcProduction = "connect-src 'self'";
        public const string ConnectSrcDevelopment = "connect-src 'self' ws: wss: http://localhost:* https://localhost:*";
        public const string ImgSrc = "img-src 'self' data:";
        public const string FrameSrc = "frame-src 'none'";
    }
}