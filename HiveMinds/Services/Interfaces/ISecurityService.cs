namespace HiveMinds.Services.Interfaces;

public interface ISecurityService
{ 
     byte[] CreateSalt(int size = 128);
     Task<byte[]> CreateHash(string password, byte[] salt);
     Task<bool> VerifyPassword(string password, byte[] salt, byte[] hash);
     string GenerateToken();
     string GenerateLoginToken();
     string GenerateCode(int length = 6);
     int GenerateId(int min = 10000, int max = 99999);
}