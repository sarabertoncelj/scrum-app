using System;

namespace smrpo_be.Data.Requests.Sprint
{
    public class SprintCreate
    {
        public Guid ProjectId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Velocity { get; set; }
    }
}
