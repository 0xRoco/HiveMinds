using Microsoft.AspNetCore.SignalR;

namespace HiveMinds.Core;

public class NotificationHub : Hub
{
    public async Task SendNotification(string title, string message, string type, bool autoClose = true,
        float timeoutDuration = 5f)
    {
        await Clients.Client(Context.ConnectionId)
            .SendAsync("ReceiveNotification", title, message, type, autoClose, timeoutDuration);
    }
}