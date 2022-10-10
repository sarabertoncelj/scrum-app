using smrpo_be.Data.Enums;
using System.Collections.Generic;
using System;

namespace smrpo_be.Data.Models
{
    public class UserStoryTime
    {
        public int Estimation { get; set; }
		public Guid UserStoryId { get; set; }
        public Guid SprintId { get; set; }

        //Relations
        public virtual UserStory UserStory { get; set; }
        public virtual Sprint Sprint { get; set; }
    }
}
