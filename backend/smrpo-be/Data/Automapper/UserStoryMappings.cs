using AutoMapper;
using smrpo_be.Data.Models;
using smrpo_be.Data.WebModels;
using smrpo_be.Data.Requests.UserStory;

namespace smrpo_be.Data.Automapper
{
    public class UserStoryMappings : Profile
    {
        public UserStoryMappings()
        {
            CreateMap<UserStoryTime, UserStoryTimeDto>();


            CreateMap<UserStory, UserStoryDto>();
            CreateMap<UserStoryDto, UserStory>();

            CreateMap<UserStoryCreate, UserStory>().ForMember(dest => dest.AcceptanceTests, opt => opt.MapFrom(so => so.AcceptanceTests));
            CreateMap<UserStoryUpdate, UserStory>().ForMember(dest => dest.AcceptanceTests, opt => opt.MapFrom(so => so.AcceptanceTests));

            CreateMap<string, AcceptanceTest>().ForMember(dest => dest.Description, opt => opt.MapFrom(so => so));
            CreateMap<AcceptanceTest, string>().ConvertUsing(source => source.Description ?? string.Empty);
        }
    }
}
