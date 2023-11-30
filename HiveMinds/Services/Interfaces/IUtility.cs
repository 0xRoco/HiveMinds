using HiveMinds.DataTypes;

namespace HiveMinds.Services.Interfaces;

public interface IUtility
{
    Task<bool> IsUserVerified(string username);

    Task<VerificationStatus> GetUserVerificationStatus(string username);
}