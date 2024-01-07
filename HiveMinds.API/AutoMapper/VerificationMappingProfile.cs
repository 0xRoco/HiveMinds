using AutoMapper;
using HiveMinds.DTO;
using HiveMinds.Models;

namespace HiveMinds.API.AutoMapper;

public class VerificationMappingProfile : Profile
{
    public VerificationMappingProfile()
    {
        CreateMap<VerificationRequest, VerificationDto>() // Set User manually
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
    }
}