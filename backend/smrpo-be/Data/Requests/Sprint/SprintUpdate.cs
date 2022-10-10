using System;

namespace smrpo_be.Data.Requests.Sprint
{
    public class SprintUpdate
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Velocity { get; set; }
    }
}
