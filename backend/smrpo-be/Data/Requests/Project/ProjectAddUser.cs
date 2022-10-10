using smrpo_be.Data.Enums;
using System;

namespace smrpo_be.Data.Requests.Project
{
    public class ProjectAddUser
    {
        public Guid Id { get; set; }
        public ProjectRole ProjectRole { get; set; }
    }
}
