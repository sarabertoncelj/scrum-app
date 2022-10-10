using System;

namespace smrpo_be.Data.Models
{
    public class WorkLog : Entity
    {
        public float HoursWorked { get; set; }
        public float HoursRemaining { get; set; }
        public DateTime Day { get; set; }

        public Guid TaskId { get; set; }
        public virtual UserStoryTask Task { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}
