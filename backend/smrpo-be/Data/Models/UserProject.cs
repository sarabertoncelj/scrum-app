using smrpo_be.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace smrpo_be.Data.Models
{
    public class UserProject
    {
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public Guid ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public IEnumerable<UserProjectRole> ProjectRoles { get; set; }

        public IEnumerable<ProjectRole> Roles 
        {
            get { return ProjectRoles.Select(x => x.Role); }
        }
    }
}
