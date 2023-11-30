using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HiveMinds.API.Services.Interfaces;
using HiveMinds.DTO;
using HiveMinds.Models;
using Konscious.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace HiveMinds.API.Services;

public class SecurityService : ISecurityService
{
    
    private readonly IConfiguration _configuration;
    private readonly ILogger<ISecurityService> _logger;

    public SecurityService(IConfiguration configuration, ILogger<ISecurityService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

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
            argon2.DegreeOfParallelism = 4;
            argon2.Iterations = 4; 
            argon2.MemorySize = 1024 * 128;
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

    public string GenerateToken(Account user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("Invalid secret key")));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddHours(1);
        
        var claims = new List<Claim>()
        {
            new(JwtRegisteredClaimNames.Sub, "TokenLogin"),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username)
        };

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
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