using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RestaurantAPI.Middleware
{
    public class RequestTimeAPI : IMiddleware
    {
        private readonly ILogger<RequestTimeAPI> _logger;
        private Stopwatch _stopwatch;

        public RequestTimeAPI(ILogger<RequestTimeAPI> logger)
        {
            _logger = logger;
            _stopwatch = new Stopwatch();
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            _stopwatch.Start();
            await next.Invoke(context);
            _stopwatch.Stop();
            var milliseconds = _stopwatch.ElapsedMilliseconds;
            if (milliseconds / 1000 > 4)
                _logger.LogInformation($"Request {context.Request.Method} at {context.Request.Path} took {_stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
