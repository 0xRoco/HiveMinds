using HiveMinds.Common;
using HiveMinds.DTO;

namespace HiveMinds.Interfaces;

public interface IUserService
{
    public Task<ApiResponse<UserDto>?> GetUser(int id);
    public Task<ApiResponse<UserDto>?> GetUser(string username);
    public Task<ApiResponse<UserDto>?> UpdateUserProfile(string username, EditProfileDto model);
    public Task<ApiResponse<object>?> VerifyEmail(VerifyEmailDto model);
}