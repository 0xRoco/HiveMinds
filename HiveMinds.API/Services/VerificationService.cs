using HiveMinds.API.Interfaces;
using HiveMinds.Common;
using HiveMinds.Database;
using HiveMinds.DTO;
using HiveMinds.Models;
using Microsoft.EntityFrameworkCore;

namespace HiveMinds.API.Services;

public class VerificationService : IVerificationService
{
    private readonly DatabaseContext _db;
    private readonly ISecurityService _securityService;
    private readonly DbSet<VerificationRequest> _verificationRequests;
    private readonly ILogger<IVerificationService> _logger;

    public enum RequestResult
    {
        Success,
        RequestNotFound,
        RequestAlreadyExists,
        InternalError
    }

    public VerificationService(DatabaseContext db, ILogger<IVerificationService> logger,
        ISecurityService securityService)
    {
        _db = db;
        _verificationRequests = _db.Set<VerificationRequest>();
        _logger = logger;
        _securityService = securityService;
    }


    public async Task<Result<VerificationRequest?, RequestResult>> GetVerificationRequestById(int id)
    {
        var request = await _verificationRequests.FindAsync(id);
        if (request == null)
        {
            return Result<VerificationRequest?, RequestResult>.Failure("Verification request not found",
                RequestResult.RequestNotFound);
        }

        return Result<VerificationRequest?, RequestResult>.Success(request);
    }

    public async Task<Result<VerificationRequest?, RequestResult>> GetVerificationRequestByUserId(int userId)
    {
        var request = await _verificationRequests.AsNoTracking().FirstOrDefaultAsync(v => v.AccountId == userId);

        if (request == null)
        {
            return Result<VerificationRequest?, RequestResult>.Failure("Verification request not found",
                RequestResult.RequestNotFound);
        }

        return Result<VerificationRequest?, RequestResult>.Success(request);
    }

    public async Task<Result<VerificationRequest?, RequestResult>> CreateVerificationRequest(
        VerificationRequestDto request)
    {
        var existingRequest = await GetVerificationRequestByUserId(request.AccountId);

        if (existingRequest.IsSuccess)
        {
            existingRequest = Result<VerificationRequest?, RequestResult>.Failure("Request already exists",
                existingRequest.Value,
                RequestResult.RequestAlreadyExists);

            return existingRequest;
        }

        var newRequest = CreateModel(request);
        await _verificationRequests.AddAsync(newRequest);
        var result = await _db.SaveChangesAsync();

        if (result == 0)
        {
            return Result<VerificationRequest?, RequestResult>.Failure("An error occurred while creating request",
                RequestResult.InternalError);
        }

        return Result<VerificationRequest?, RequestResult>.Success(newRequest);
    }

    private VerificationRequest CreateModel(VerificationRequestDto dto)
    {
        return new VerificationRequest
        {
            Id = _securityService.GenerateId(),
            AccountId = dto.AccountId,
            Reason = dto.Reason,
            Status = VerificationStatus.Pending,
            CreatedAt = default
        };
    }
}