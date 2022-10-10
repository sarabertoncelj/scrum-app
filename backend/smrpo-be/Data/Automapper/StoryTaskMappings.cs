using AutoMapper;
using smrpo_be.Data.Models;
using smrpo_be.Data.Requests.UserStory;
using smrpo_be.Data.WebModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace smrpo_be.Data.Automapper
{
    public class StoryTaskMappings : Profile
    {

        public StoryTaskMappings()
        {
            CreateMap<UserStoryTask, StoryTaskDto>();
            CreateMap<TaskCreate, UserStoryTask>();

            CreateMap<WorkLog, WorkLogDto>();
        }
    }
}
