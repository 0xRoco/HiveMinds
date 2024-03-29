using AutoMapper;
using HiveMinds.DTO;
using HiveMinds.Models;

namespace HiveMinds.Core.AutoMapper;

public class LikeMappingProfile : Profile
{
    public LikeMappingProfile()
    {
        CreateMap<LikeDto,  ThoughtLike>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ThoughtId, opt => opt.MapFrom(src => src.ThoughtId))
            .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.User.Id))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
    }
}