using AutoMapper;
using smrpo_be.Data.Models;
using smrpo_be.Data.Requests.Sprint;
using smrpo_be.Data.WebModels;
using System;

namespace smrpo_be.Data.Automapper
{
    public class SprintMappings : Profile
    {
        public SprintMappings()
        {
            CreateMap<Sprint, SprintDto>()
                .ForMember(dest => dest.Active, opt => opt.MapFrom(so => so.Start <= DateTime.Now && so.End >= DateTime.Now));
            CreateMap<Sprint, SprintMetaDto>()
                .ForMember(dest => dest.Active, opt => opt.MapFrom(so => so.Start <= DateTime.Now && so.End >= DateTime.Now));
            CreateMap<SprintDto, Sprint>();
            CreateMap<SprintMetaDto, Sprint>();
            CreateMap<SprintCreate, Sprint>();
        }
    }
}
