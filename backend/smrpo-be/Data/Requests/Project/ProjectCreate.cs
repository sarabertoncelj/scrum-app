using System.Collections.Generic;

namespace smrpo_be.Data.Requests.Project
{
    public class ProjectCreate
    {
        public string Name { get; set; }
        public IEnumerable<ProjectAddUser> ProjectUsers { get; set; }
    }
}
