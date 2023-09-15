namespace HiveMinds.Services.Interfaces;

public interface ISecurityService
{
    public byte[] CreateSalt();
    public Task<byte[]> CreateHash(string password, byte[] salt);
    public Task<bool> VerifyPassword(string password, byte[] salt, byte[] hash);
    public string GenerateToken();
    public string GenerateLoginToken();
    public string GenerateCode(int length = 6);
    public int GenerateId(int min = 10000, int max = 99999);
}