using System;

namespace smrpo_be.Data.Requests
{
    public class CreateWorkLog
    {
        public float HoursWorked { get; set; }
        public float HoursRemaining { get; set; }
        public DateTime Day { get; set; }
        public Guid UserId { get; set; }
    }
}
