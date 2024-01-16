using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HiveMinds.Models;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;
using HiveMinds.API.Interfaces;

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

    public string CreatePasswordHash(string password, int cost = 9, HashType hashType = HashType.SHA512)
    {
        var passwordHash = BC.EnhancedHashPassword(password, cost, hashType);
        return passwordHash;
    }

    public bool VerifyPasswordHash(string password, string hash, HashType hashType = HashType.SHA512)
    {
        return BC.EnhancedVerify(password, hash, hashType);
    }

    public string GenerateToken(Account user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("Invalid secret key")));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddHours(1);
        
        var claims = new List<Claim>()
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
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