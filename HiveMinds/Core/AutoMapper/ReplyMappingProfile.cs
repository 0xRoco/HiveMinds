using AutoMapper;
using HiveMinds.DTO;
using HiveMinds.Models;
using HiveMinds.ViewModels;

namespace HiveMinds.Core.AutoMapper;

public class ReplyMappingProfile : Profile
{
    public ReplyMappingProfile()
    {
        CreateMap<ThoughtReply, ReplyViewModel>() // Set Username manually
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.parentId, opt => opt.MapFrom(src => src.ThoughtId))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

        CreateMap<ReplyDto, ReplyViewModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.parentId, opt => opt.MapFrom(src => src.ThoughtId))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
    }
}