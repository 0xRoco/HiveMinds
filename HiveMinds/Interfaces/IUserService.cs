using HiveMinds.Common;
using HiveMinds.DTO;

namespace HiveMinds.Interfaces;

public interface IUserService
{
    public Task<ApiResponse<UserDto>?> GetUser(int id);
    public Task<ApiResponse<UserDto>?> GetUser(string username);

    public Task<ApiResponse<UserDto>?> UpdateUserProfile(string username, EditProfileDto model,
        IFormFile? profilePicture);
    public Task<ApiResponse<object>?> VerifyEmail(VerifyEmailDto model);


    public Task<ApiResponse<VerificationDto>?> GetVerificationRequest(int accountId);
    public Task<ApiResponse<VerificationDto>?> GetVerificationRequest(string username);
    public Task<ApiResponse<VerificationDto>?> CreateVerificationRequest(VerificationRequestDto request);
}