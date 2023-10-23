using AutoMapper;
using HiveMinds.Models;
using HiveMinds.ViewModels;

namespace HiveMinds.Common.AutoMapper;

public class ThoughtMappingProfile : Profile
{
    public ThoughtMappingProfile()
    {
        CreateMap<Thought, ThoughtViewModel>() // Set Username, Likes, and Replies manually
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
    }
}