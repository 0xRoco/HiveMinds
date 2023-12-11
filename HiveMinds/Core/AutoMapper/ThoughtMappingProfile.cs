using AutoMapper;
using HiveMinds.DTO;
using HiveMinds.Models;
using HiveMinds.ViewModels;

namespace HiveMinds.Core.AutoMapper;

public class ThoughtMappingProfile : Profile
{
    public ThoughtMappingProfile()
    {
        CreateMap<Thought, ThoughtViewModel>() // Set Username, Likes, and Replies manually
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));


        CreateMap<ThoughtDto, ThoughtViewModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.User))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.Likes))
            .ForMember(dest => dest.Replies, opt => opt.MapFrom(src => src.Replies))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
    }
}