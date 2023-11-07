using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Contracts.Security;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Monitoring
{
    public class RequestPerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<TRequest> _logger;
        private readonly Stopwatch _timer;

        public RequestPerformanceBehaviour(
            ILogger<TRequest> logger,
            ICurrentUserService currentUserService)
        {
            _timer = new Stopwatch();

            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            if (_timer.ElapsedMilliseconds > 500)
            {
                var userId = _currentUserService.GetCurrentUserId();
                _logger.LogWarning(
                    "Cronos Long Running Request: {@Request} ({ElapsedMilliseconds} milliseconds) by {UserId}",
                    request,
                    _timer.ElapsedMilliseconds,
                    userId);
            }

            return response;
        }
    }
}