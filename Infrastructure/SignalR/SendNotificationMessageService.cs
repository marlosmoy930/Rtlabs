namespace Infrastructure.SignalR
{
    // public class SendNotificationMessageService : ISendNotificationMessageService
    // {
    //     private readonly ILogger<NotificationMessageHub> _logger;
    //     private readonly IHubContext<NotificationMessageHub> _hubContext;
    //     private readonly ICurrentUserService _currentUserService;
    //
    //     public SendNotificationMessageService(
    //         IHubContext<NotificationMessageHub> hubContext,
    //         ILogger<NotificationMessageHub> logger,
    //         ICurrentUserService currentUserService)
    //     {
    //         _hubContext = hubContext;
    //         _logger = logger;
    //         _currentUserService = currentUserService;
    //     }
    //
    //     private void CheckHubContextState(string groupName = default(string))
    //     {
    //         if (_hubContext == null)
    //         {
    //             _logger.LogError("PushNotificationMessageExtensions. hubContext == null");
    //             return;
    //         }
    //
    //         if (_hubContext.Clients == null)
    //         {
    //             _logger.LogWarning("PushNotificationMessageExtensions. hubContext.Clients == null");
    //             return;
    //         }
    //
    //         if (groupName != null && groupName.Length > 0)
    //         {
    //             if (_hubContext.Clients.Group(groupName) == null)
    //             {
    //                 _logger.LogWarning("PushNotificationMessageExtensions. hubContext.Clients.Group(groupName) == null for {groupName}", groupName);
    //                 return;
    //             }
    //         }
    //     }
    // }
}
