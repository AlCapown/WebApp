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
                CspDirectives.FrameSrc,
                CspDirectives.TrustedTypes
            })
            : string.Join("; ", new[]
            {
                CspDirectives.DefaultSrc,
                CspDirectives.ScriptSrc,
                CspDirectives.StyleSrc,
                CspDirectives.FontSrc,
                CspDirectives.ConnectSrcProduction,
                CspDirectives.ImgSrc,
                CspDirectives.FrameSrc,
                CspDirectives.TrustedTypes
            });
    }

    public Task InvokeAsync(HttpContext context, RequestDelegate requestDelegate)
    {
        // Security Headers
        context.Response.Headers[HeaderNames.XFrameOptions] = HeaderValues.XFrameOptionsDeny;
        context.Response.Headers[HeaderNames.XContentTypeOptions] = HeaderValues.XContentTypeOptionsNoSniff;
        context.Response.Headers[HeaderNames.ReferrerPolicy] = HeaderValues.ReferrerPolicyStrictOrigin;
        context.Response.Headers[HeaderNames.ContentSecurityPolicy] = _contentSecurityPolicy;

        // Cross-origin headers for WASM multithreading support (for when its is finally implemented in the future).
        context.Response.Headers[HeaderNames.CrossOriginEmbedderPolicy] = HeaderValues.CrossOriginEmbedderPolicyRequireCorp;
        context.Response.Headers[HeaderNames.CrossOriginOpenerPolicy] = HeaderValues.CrossOriginOpenerPolicySameOrigin;

        // HSTS - Strict Transport Security
        if (_environment.IsProduction())
        {
            context.Response.Headers[HeaderNames.StrictTransportSecurity] = HeaderValues.StrictTransportSecurity;
        }

        return requestDelegate(context);
    }

    private static class HeaderNames
    {
        /// <summary>
        /// Controls whether the page can be displayed in a frame, iframe, embed or object.
        /// Used to prevent clickjacking attacks by restricting how the page can be embedded.
        /// </summary>
        public const string XFrameOptions = "X-Frame-Options";
        
        /// <summary>
        /// Prevents MIME type sniffing and forces browsers to use the declared content-type.
        /// Helps prevent attacks where malicious content is disguised as harmless files.
        /// </summary>
        public const string XContentTypeOptions = "X-Content-Type-Options";
        
        /// <summary>
        /// Controls how much referrer information is sent along with requests.
        /// Helps protect user privacy by limiting what information is shared with external sites.
        /// </summary>
        public const string ReferrerPolicy = "Referrer-Policy";
        
        /// <summary>
        /// Defines approved sources of content that browsers may load (prevents XSS attacks).
        /// Acts as a allowlist for scripts, styles, images, and other resources.
        /// </summary>
        public const string ContentSecurityPolicy = "Content-Security-Policy";
        
        /// <summary>
        /// Controls the fetching of cross-origin resources to enable features like SharedArrayBuffer.
        /// Required for advanced WebAssembly features and cross-origin isolation.
        /// </summary>
        public const string CrossOriginEmbedderPolicy = "Cross-Origin-Embedder-Policy";
        
        /// <summary>
        /// Isolates the browsing context from cross-origin documents.
        /// Provides additional security by preventing certain cross-origin interactions.
        /// </summary>
        public const string CrossOriginOpenerPolicy = "Cross-Origin-Opener-Policy";
        
        /// <summary>
        /// Forces browsers to use HTTPS connections and protects against protocol downgrade attacks.
        /// Once set, browsers will automatically redirect HTTP requests to HTTPS for the specified duration.
        /// </summary>
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
        public const string ScriptSrc = "script-src 'self' 'unsafe-eval' 'wasm-unsafe-eval'";
        public const string StyleSrc = "style-src 'self' 'unsafe-inline' fonts.googleapis.com";
        public const string FontSrc = "font-src 'self' fonts.gstatic.com";
        public const string ConnectSrcProduction = "connect-src 'self'";
        public const string ConnectSrcDevelopment = "connect-src 'self' ws: wss: http://localhost:* https://localhost:*";
        public const string ImgSrc = "img-src 'self' data:";
        public const string FrameSrc = "frame-src 'none'";
        public const string TrustedTypes = "trusted-types 'none'";
    }
}