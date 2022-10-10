using smrpo_be.Data.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace smrpo_be.Data.Models
{
    public class UserStoryTask : Entity
    {
        public string Description { get; set; }
        public float RemainingTime { get; set; }
        public DateTime ActiveFrom { get; set; }
        public bool Accepted { get; set; }
        public UserStoryTaskStatus Status { get; set; }

        // Relations
        public Guid UserStoryId { get; set; }
        public virtual UserStory UserStory { get; set; }
        public Guid? UserId { get; set; }
        public virtual User User { get; set; }

        public virtual IEnumerable<WorkLog> WorkLogs { get; set; }
    }
}
