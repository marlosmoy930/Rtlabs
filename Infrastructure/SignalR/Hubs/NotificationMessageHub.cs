using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Infrastructure.SignalR.Hubs
{
    public class NotificationMessageHub : Hub
    {
        private readonly ILogger<NotificationMessageHub> _logger;

        public NotificationMessageHub(ILogger<NotificationMessageHub> logger)
        {
            _logger = logger;
        }

        public async Task Subscribe(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task Unsubscribe(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                throw new ArgumentNullException($"{nameof(groupName)} can not be a null or empty");
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"ConnectionID: {Context.ConnectionId}, User: {Context.User.Identity.Name}. CONNECTED");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation($"ConnectionID: {Context.ConnectionId}, User: {Context.User.Identity.Name}. DISCONNECTED");
            await base.OnDisconnectedAsync(exception);
        }
    }
}