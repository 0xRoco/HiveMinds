using AutoMapper;
using HiveMinds.DTO;
using HiveMinds.Models;

namespace HiveMinds.API.AutoMapper;

public class LikeMappingProfile : Profile
{
    public LikeMappingProfile()
    {
        CreateMap<ThoughtLike, LikeDto>() // Set User manually
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ThoughtId, opt => opt.MapFrom(src => src.ThoughtId))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
    }
}