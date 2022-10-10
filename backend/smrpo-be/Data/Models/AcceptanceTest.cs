using System;

namespace smrpo_be.Data.Models
{
    public class AcceptanceTest : Entity
    {
        public string Description { get; set; }

        //Relations
		public Guid UserStoryId { get; set; }
        public virtual UserStory UserStory { get; set; }
    }
}
