using AutoMapper;
using HiveMinds.DTO;
using HiveMinds.Models;
using HiveMinds.ViewModels;

namespace HiveMinds.API.AutoMapper;

public class ReplyMappingProfile : Profile
{
    public ReplyMappingProfile()
    {
        CreateMap<ThoughtReply, ReplyDto>() // Set User manually
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ThoughtId, opt => opt.MapFrom(src => src.ThoughtId))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
    }
}