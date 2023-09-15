namespace HiveMinds.Services.Interfaces;

public interface IAdminService
{
    Task PurgeAllUsers();
    Task<bool> BanUser(string username);
    Task<bool> UnbanUser(string username);
    Task<bool> VerifyUser(string username);
    Task<bool> UnVerifyUser(string username);

    Task<bool> PurgeAllBans();
    Task PurgeAllThoughts();
}