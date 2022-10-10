using System;

namespace smrpo_be.Data.Requests.UserStory
{
    public class TaskCreate
    {
        public string Description { get; set; }
        public float RemainingTime { get; set; }
        public Guid UserStoryId { get; set; }
        public Guid? UserId { get; set; }
    }
}
