using System;

namespace smrpo_be.Data.Requests.UserStory
{
    public class UserStoryTimeUpdate
    {
        public int Estimation { get; set; }
        public Guid SprintId { get; set; }
    }
}
