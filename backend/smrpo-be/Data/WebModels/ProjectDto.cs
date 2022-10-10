using smrpo_be.Data.Enums;
using System.Collections.Generic;

namespace smrpo_be.Data.WebModels
{
    public class ProjectDto : EntityDto
    {
        public string Name { get; set; }
        public IEnumerable<ProjectRole> ProjectRoles { get; set; }

        public IEnumerable<ProjectUserDto> Users { get; set; }
    }

    public class ProjectUserDto : EntityDto
    {
        public string Username { get; set; }
        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public IEnumerable<ProjectRole> ProjectRoles { get; set; }
    }
}
