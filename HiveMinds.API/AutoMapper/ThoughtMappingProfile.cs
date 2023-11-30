using AutoMapper;
using HiveMinds.DTO;
using HiveMinds.Models;

namespace HiveMinds.API.AutoMapper;

public class ThoughtMappingProfile : Profile
{
    public ThoughtMappingProfile()
    {
        CreateMap<Thought, ThoughtDto>() // Set User, Replies and likes manually
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
    }
}