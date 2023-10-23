using AutoMapper;
using HiveMinds.Models;
using HiveMinds.ViewModels;

namespace HiveMinds.Common.AutoMapper;

public class ReplyMappingProfile : Profile
{
    public ReplyMappingProfile()
    {
        CreateMap<ThoughtReply, ThoughtReplyViewModel>() // Set Username manually
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.parentId, opt => opt.MapFrom(src => src.ThoughtId))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
    }
}