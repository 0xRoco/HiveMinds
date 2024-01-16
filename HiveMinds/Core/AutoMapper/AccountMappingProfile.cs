using AutoMapper;
using HiveMinds.DTO;
using HiveMinds.ViewModels;

namespace HiveMinds.Core.AutoMapper;

public class AccountMappingProfile : Profile
{
    public AccountMappingProfile()
    {
        CreateMap<UserDto, UserViewModel>()
            .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.ProfilePictureUrl))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
            .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => src.IsVerified))
            .ForMember(dest => dest.Joined, opt => opt.MapFrom(src => src.CreatedAt));
    }
}