using AutoMapper;
using HiveMinds.DTO;
using HiveMinds.Models;

namespace HiveMinds.API.AutoMapper;

public class AccountMappingProfile : Profile
{
    public AccountMappingProfile()
    {
        CreateMap<Account, UserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.ProfilePictureUrl, opt => opt.MapFrom(src => src.ProfilePictureUrl))
            .ForMember(dest => dest.LoyaltyStatement, opt => opt.MapFrom(src => src.LoyaltyStatement))
            .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
            .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => src.IsVerified))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
        
        CreateMap<Account, AccountDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.ProfilePictureUrl, opt => opt.MapFrom(src => src.ProfilePictureUrl))
            .ForMember(dest => dest.LoyaltyStatement, opt => opt.MapFrom(src => src.LoyaltyStatement))
            .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
            .ForMember(dest => dest.VerificationRequest, opt => opt.MapFrom(src => src.VerificationRequest))
            .ForMember(dest => dest.IsEmailVerified, opt => opt.MapFrom(src => src.IsEmailVerified))
            .ForMember(dest => dest.IsPhoneNumberVerified, opt => opt.MapFrom(src => src.IsPhoneNumberVerified))
            .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => src.IsVerified))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
    }
}