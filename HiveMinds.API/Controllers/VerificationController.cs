using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using AutoMapper;
using HiveMinds.API.Core;
using HiveMinds.API.Interfaces;
using HiveMinds.API.Services;
using HiveMinds.Common;
using HiveMinds.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;

namespace HiveMinds.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VerificationController : ControllerBase
{
    private readonly IAccountRepository _accountRepository;
    private readonly IVerificationService _verificationService;
    private readonly IMapper _mapper;
    private readonly ILogger<VerificationController> _logger;

    public VerificationController(IAccountRepository accountRepository, IMapper mapper,
        IVerificationService verificationService, ILogger<VerificationController> logger)
    {
        _accountRepository = accountRepository;
        _mapper = mapper;
        _verificationService = verificationService;
        _logger = logger;
    }

    [HttpGet("{id:int}")]
    public async Task<ApiResponse<VerificationDto>> GetById(int id)
    {
        if (!ModelState.IsValid)
            return ApiResponse<VerificationDto>.FailureResponse(HttpStatusCode.BadRequest, "Invalid request");

        var idFromToken = Utility.GetAccountIdFromClaims(User);

        try
        {
            var requestResult = await _verificationService.GetVerificationRequestById(id);

            if (!requestResult.IsSuccess || requestResult.Value == null)
            {
                return ApiResponse<VerificationDto>.FailureResponse(HttpStatusCode.NotFound,
                    "Verification request not found");
            }

            if (requestResult.Value.AccountId != idFromToken)
            {
                return ApiResponse<VerificationDto>.FailureResponse(HttpStatusCode.BadRequest,
                    "Invalid account id");
            }

            var request = requestResult.Value;

            var dto = _mapper.Map<VerificationDto>(request);
            var user = await _accountRepository.GetById(request.AccountId);
            dto.User = _mapper.Map<UserDto>(user);

            return ApiResponse<VerificationDto>.SuccessResponse("Verification request found", dto);
        }
        catch (Exception e)
        {
            _logger.LogError("Error getting verification request by id: {id}\n Exception: {ex}",
                id, e.ToString());

            return ApiResponse<VerificationDto>.FailureResponse(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    [HttpGet]
    public async Task<ApiResponse<VerificationDto>> GetByUserId([Required] int accountId)
    {
        if (!ModelState.IsValid)
            return ApiResponse<VerificationDto>.FailureResponse(HttpStatusCode.BadRequest, "Invalid request");

        var idFromToken = Utility.GetAccountIdFromClaims(User);

        try
        {
            if (accountId != idFromToken)
            {
                return ApiResponse<VerificationDto>.FailureResponse(HttpStatusCode.BadRequest,
                    "Invalid user id");
            }

            var requestResult = await _verificationService.GetVerificationRequestByUserId(accountId);

            if (!requestResult.IsSuccess || requestResult.Value == null)
            {
                return ApiResponse<VerificationDto>.FailureResponse(HttpStatusCode.NotFound,
                    "Verification request not found");
            }

            var request = requestResult.Value;


            var dto = _mapper.Map<VerificationDto>(request);
            var user = await _accountRepository.GetById(request.AccountId);
            dto.User = _mapper.Map<UserDto>(user);

            return ApiResponse<VerificationDto>.SuccessResponse("Verification request found", dto);
        }
        catch (Exception e)
        {
            _logger.LogError("Error getting verification request by user id: {userId}\n Exception: {ex}",
                accountId, e.ToString());

            return ApiResponse<VerificationDto>.FailureResponse(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    [HttpPost]
    public async Task<ApiResponse<VerificationDto>> Create([FromBody] VerificationRequestDto request)
    {
        if (!ModelState.IsValid)
            return ApiResponse<VerificationDto>.FailureResponse(HttpStatusCode.BadRequest, "Invalid request");

        var idFromToken = Utility.GetAccountIdFromClaims(User);
        try
        {
            if (request.AccountId != idFromToken)
            {
                return ApiResponse<VerificationDto>.FailureResponse(HttpStatusCode.BadRequest,
                    "Invalid account id");
            }

            var user = await _accountRepository.GetById(idFromToken);

            if (user == null)
            {
                return ApiResponse<VerificationDto>.FailureResponse(HttpStatusCode.NotFound,
                    "User not found");
            }

            var requestResult = await _verificationService.CreateVerificationRequest(request);

            if (requestResult is { Status: VerificationService.RequestResult.RequestAlreadyExists, Value: not null })
            {
                var existingDto = _mapper.Map<VerificationDto>(requestResult.Value);
                user = await _accountRepository.GetById(requestResult.Value.AccountId);
                existingDto.User = _mapper.Map<UserDto>(user);

                return ApiResponse<VerificationDto>.FailureResponse(HttpStatusCode.BadRequest,
                    "Verification request already exists", existingDto);
            }

            if (!requestResult.IsSuccess || requestResult.Value == null)
                return ApiResponse<VerificationDto>.FailureResponse(HttpStatusCode.InternalServerError,
                    requestResult.Message);

            var result = requestResult.Value;

            var dto = _mapper.Map<VerificationDto>(result);
            user = await _accountRepository.GetById(result.AccountId);
            dto.User = _mapper.Map<UserDto>(user);

            return ApiResponse<VerificationDto>.SuccessResponse("Verification request created", dto);
        }
        catch (Exception e)
        {
            _logger.LogError("Error creating verification request: {request}\n Exception: {ex}",
                request.ToJson(Formatting.Indented), e.ToString());
            return ApiResponse<VerificationDto>.FailureResponse(HttpStatusCode.InternalServerError,
                e.Message);
        }
    }
}