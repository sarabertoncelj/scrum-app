using smrpo_be.Data.Enums;
using System.Collections.Generic;
using System;

namespace smrpo_be.Data.Models
{
    public class UserStory : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public UserStoryPriority Priority { get; set; }
        public int BusinessValue { get; set; }
		public Guid ProjectId { get; set; }
        public Guid? SprintId { get; set; }
        public UserStoryStatus Status { get; set; }
        public string Comment { get; set; }
        public bool Deleted { get; set; }

        //Relations
        public virtual Project Project { get; set; }
        public virtual IEnumerable<AcceptanceTest> AcceptanceTests { get; set; }
        public virtual IEnumerable<UserStoryTime> UserStoryTimes { get; set; }
        public virtual Sprint Sprint { get; set; }
        public virtual IEnumerable<UserStoryTask> Tasks { get; set; }
    }
}
