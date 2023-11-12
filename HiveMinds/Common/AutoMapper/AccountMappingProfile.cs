using AutoMapper;
using HiveMinds.DTO;
using HiveMinds.Models;
using HiveMinds.ViewModels;

namespace HiveMinds.Common.AutoMapper;

public class AccountMappingProfile : Profile
{
    public AccountMappingProfile()
    {
        CreateMap<Account, UserViewModel>() // Set Likes and Thoughts manually
            .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.ProfilePictureUrl))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.PartyLoyaltyStatement, opt => opt.MapFrom(src => src.LoyaltyStatement))
            .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
            .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => src.IsVerified))
            .ForMember(dest => dest.Joined, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.LastSeen, opt => opt.MapFrom(src => src.LastLoginAt));
            
    }
}