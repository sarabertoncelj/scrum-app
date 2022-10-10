using System;
using System.Threading.Tasks;

namespace smrpo_be.Data.WebModels
{
    public class StoryTaskDto : EntityDto
    {
        public string Description { get; set; }
        public float RemainingTime { get; set; }
        public DateTime ActiveFrom { get; set; }
        public bool Accepted { get; set; }
        public TaskStatus Status { get; set; }
        public UserDto User { get; set; }
        public Guid UserStoryId { get; set; }
    }
}
