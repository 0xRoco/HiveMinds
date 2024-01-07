using System.ComponentModel;
using System.Net;
using AutoMapper;
using HiveMinds.API.Interfaces;
using HiveMinds.Common;
using HiveMinds.Models;
using Microsoft.AspNetCore.Mvc;

namespace HiveMinds.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VerificationController : ControllerBase
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;

    public VerificationController(IAccountRepository accountRepository, IMapper mapper)
    {
        _accountRepository = accountRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ApiResponse<IEnumerable<VerificationRequest>>> Get()
    {
        var requests = await _accountRepository.GetVerificationRequests();
        var dto = _mapper.Map<IEnumerable<VerificationRequest>>(requests);

        return ApiResponse<IEnumerable<VerificationRequest>>.SuccessResponse("Verification requests found", requests);
    }

    [HttpPost]
    public async Task<ApiResponse<VerificationRequest>> Create([FromBody] VerificationRequest request)
    {
        if (!ModelState.IsValid)
            return ApiResponse<VerificationRequest>.FailureResponse(HttpStatusCode.BadRequest, "Invalid request");
        try
        {
            var result = await _accountRepository.CreateVerificationRequest(request);
            return result
                ? ApiResponse<VerificationRequest>.SuccessResponse("Verification request created", request)
                : ApiResponse<VerificationRequest>.FailureResponse(HttpStatusCode.InternalServerError,
                    "Failed to create verification request");
        }
        catch (Exception e)
        {
            return ApiResponse<VerificationRequest>.FailureResponse(HttpStatusCode.InternalServerError,
                e.Message);
        }
    }
}