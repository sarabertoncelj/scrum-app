using AutoMapper;
using smrpo_be.Data.Models;
using smrpo_be.Data.Requests.User;
using smrpo_be.Data.WebModels;

namespace smrpo_be.Data.Automapper
{
    public class UserMappings : Profile
    {
        public UserMappings()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();

            CreateMap<UserRegistration, User>();
            CreateMap<UserUpdate, User>();

            CreateMap<User, UserSearchableDto>();
        }
    }
}
