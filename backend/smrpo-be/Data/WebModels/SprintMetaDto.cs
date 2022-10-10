using System;
using System.Collections.Generic;

namespace smrpo_be.Data.WebModels
{
    public class SprintMetaDto : EntityDto
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Velocity { get; set; }
        public bool Active { get; set; }
    }
}
