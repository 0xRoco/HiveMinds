using HiveMinds.Core;
using HiveMinds.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace HiveMinds.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task Success(string title, string message, bool autoClose = true, float timeoutDuration = 5)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", title, message, "success", autoClose,
            timeoutDuration);
    }

    public async Task Error(string title, string message, bool autoClose = true, float timeoutDuration = 5)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", title, message, "danger", autoClose,
            timeoutDuration);
    }

    public async Task Info(string title, string message, bool autoClose = true, float timeoutDuration = 5)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", title, message, "info", autoClose,
            timeoutDuration);
    }

    public async Task Warning(string title, string message, bool autoClose = true, float timeoutDuration = 5)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", title, message, "warning", autoClose,
            timeoutDuration);
    }
}