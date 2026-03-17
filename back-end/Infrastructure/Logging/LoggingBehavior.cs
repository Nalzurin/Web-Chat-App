using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace back_end.Infrastructure.Logging;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var sanitized = Sanitize(request);
        _logger.LogInformation("Handling request {RequestName} {@Request}", requestName, sanitized);

        var sw = Stopwatch.StartNew();
        try
        {
            var response = await next();
            sw.Stop();
            _logger.LogInformation("Handled request {RequestName} in {ElapsedMilliseconds}ms", requestName, sw.ElapsedMilliseconds);
            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "Request {RequestName} failed after {ElapsedMilliseconds}ms", requestName, sw.ElapsedMilliseconds);
            throw;
        }
    }

    private static object? Sanitize(object? request)
    {
        if (request == null)
            return null;

        // Primitive or string -> return as-is
        var type = request.GetType();
        if (type.IsPrimitive || request is string || request is DateTime || request is Guid)
            return request;

        try
        {
            var dict = new Dictionary<string, object?>();
            var props = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var p in props)
            {
                object? val = null;
                try { val = p.GetValue(request); } catch { val = null; }
                if (p.Name.IndexOf("password", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    dict[p.Name] = "***REDACTED***";
                }
                else
                {
                    dict[p.Name] = val;
                }
            }

            return dict;
        }
        catch
        {
            return "<unserializable-request>";
        }
    }
}
