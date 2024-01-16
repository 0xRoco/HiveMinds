namespace HiveMinds.Interfaces;

public interface INotificationService
{
    Task Success(string title, string message, bool autoClose = true, float timeoutDuration = 5f);
    Task Error(string title, string message, bool autoClose = true, float timeoutDuration = 5f);
    Task Info(string title, string message, bool autoClose = true, float timeoutDuration = 5f);
    Task Warning(string title, string message, bool autoClose = true, float timeoutDuration = 5f);
}