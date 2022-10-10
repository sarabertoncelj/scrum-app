using System;

namespace smrpo_be.Data.WebModels
{
    public class WorkLogDto : EntityDto
    {
        public float HoursWorked { get; set; }
        public float HoursRemaining { get; set; }
        public DateTime Day { get; set; }
        public Guid UserId { get; set; }
    }
}
