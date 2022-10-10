using AutoMapper;
using smrpo_be.Data.Models;
using smrpo_be.Data.WebModels;
using smrpo_be.Data.Requests.UserStory;

namespace smrpo_be.Data.Automapper
{
    public class UserStoryTimeMappings : Profile
    {
        public UserStoryTimeMappings()
        {
            CreateMap<UserStoryTime, UserStoryTimeDto>();
            CreateMap<UserStoryTimeDto, UserStoryTime>();
        }
    }
}
