using BCrypt.Net;
using HiveMinds.Models;

namespace HiveMinds.API.Interfaces;

public interface ISecurityService
{
     string CreatePasswordHash(string password, int cost = 9, HashType hashType = HashType.SHA512);
     bool VerifyPasswordHash(string password, string hash, HashType hashType = HashType.SHA512);
     string GenerateToken(Account user);
     string GenerateCode(int length = 6);
     int GenerateId(int min = 100000000, int max = 999999999);
}