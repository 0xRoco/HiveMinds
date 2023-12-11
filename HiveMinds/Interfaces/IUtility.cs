using HiveMinds.Common;

namespace HiveMinds.Interfaces;

public interface IUtility
{
    Task<bool> IsUserVerified(string username);

    Task<VerificationStatus> GetUserVerificationStatus(string username);
}