using HiveMinds.DTO;

namespace HiveMinds.Services.Interfaces;

public interface IUserService
{
    public Task<UserDto?> GetUser(int id);
    public Task<UserDto?> GetUser(string username);
}