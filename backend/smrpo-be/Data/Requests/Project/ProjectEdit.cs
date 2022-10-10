using System.Collections.Generic;

namespace smrpo_be.Data.Requests.Project
{
    public class ProjectEdit
    {
        public string Name { get; set; }
        public IEnumerable<ProjectAddUser> ProjectUsers { get; set; }
    }
}
