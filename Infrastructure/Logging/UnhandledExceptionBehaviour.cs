using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Exceptions;
using Infrastructure.Contracts.Security;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Logging
{
    public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<TRequest> _logger;
        //private readonly ILongRunOperationService _longRunOperationService;

        public UnhandledExceptionBehaviour(
            ILogger<TRequest> logger,
            ICurrentUserService currentUserService)
            //ILongRunOperationService longRunOperationService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
            //_longRunOperationService = longRunOperationService;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                return await next();
            }
            catch (BusinessException bex)
            {
                var userId = _currentUserService.GetCurrentUserId();

                _logger.LogWarning(bex, "Cronos Request Business Exception: {@Request} by {UserId}", request, userId);

                if (bex.ShouldResetLongRunOperation)
                {
                    //await ResetLongOperationStatus(request);
                }

                throw;
            }
            catch (Exception ex)
            {
                var userId = _currentUserService.GetCurrentUserId();

                _logger.LogError(ex, "Cronos Request Unhandled Exception: {@Request} by {UserId}", request, userId);

                //await ResetLongOperationStatus(request);

                throw;
            }
        }
    }
}