using HiveMinds.API.Services;
using HiveMinds.Common;
using HiveMinds.DTO;
using HiveMinds.Models;

namespace HiveMinds.API.Interfaces;

public interface IVerificationService
{
    public Task<Result<VerificationRequest?, VerificationService.RequestResult>> GetVerificationRequestById(int id);

    public Task<Result<VerificationRequest?, VerificationService.RequestResult>>
        GetVerificationRequestByUserId(int userId);

    public Task<Result<VerificationRequest?, VerificationService.RequestResult>> CreateVerificationRequest(
        VerificationRequestDto request);
}