using System.Security.Cryptography;
using System.Text;
using HiveMinds.Services.Interfaces;
using Konscious.Security.Cryptography;

namespace HiveMinds.Services;

public class SecurityService : ISecurityService
{
    public byte[] CreateSalt(int size = 128)
    {
        var buffer = new byte[size];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(buffer);
        return buffer;
    }

    public async Task<byte[]> CreateHash(string password, byte[] salt)
    {
        byte[] result;
        using (var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))){

            argon2.Salt = salt;
            argon2.DegreeOfParallelism = 8;
            argon2.Iterations = 7; // 0.75 * (RAM / CCU) --> 0.75 * (1000 / 100) = 7.5
            argon2.MemorySize = 1024 * 1024; // 1 GB
            result = await argon2.GetBytesAsync(128);
        }
        GC.Collect();
        return result;
    }

    public async Task<bool> VerifyPassword(string password, byte[] salt, byte[] hash)
    {
        var newHash = await CreateHash(password, salt);
        return hash.SequenceEqual(newHash);
    }

    public string GenerateToken()
    {
        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        return token;
    }

    public string GenerateLoginToken()
    {
        return "";
    }

    public string GenerateCode(int length = 6)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[RandomNumberGenerator.GetInt32(s.Length)]).ToArray());
    }

    public int GenerateId(int min = 10000, int max = 99999)
    {
        return RandomNumberGenerator.GetInt32(min, max);
    }
}