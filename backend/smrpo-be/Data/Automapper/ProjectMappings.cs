using AutoMapper;
using smrpo_be.Data.Models;
using smrpo_be.Data.Requests.Project;
using smrpo_be.Data.WebModels;
using System.Linq;

namespace smrpo_be.Data.Automapper
{
    public class ProjectMappings : Profile
    {
        public ProjectMappings()
        {
            CreateMap<Project, ProjectDto>()
                .ForMember(dest => dest.Users, opt => opt.MapFrom(so => so.Users));

            CreateMap<ProjectCreate, Project>();
            CreateMap<ProjectEdit, Project>();

            CreateMap<User, ProjectUserDto>();

            CreateMap<ProjectPost, ProjectPostDto>();

            CreateMap<ProjectAddPost, ProjectPost>();

            CreateMap<UserProject, ProjectUserDto>()
                .ForMember(dest => dest.ProjectRoles, opt => opt.MapFrom(so => so.ProjectRoles.Select(x => x.Role)))
                .IncludeMembers(s => s.User);

            CreateMap<UserProject, ProjectDto>()
                .ForMember(dest => dest.ProjectRoles, opt => opt.MapFrom(so => so.ProjectRoles.Select(x => x.Role)))
                .IncludeMembers(s => s.Project);
        }
    }
}
