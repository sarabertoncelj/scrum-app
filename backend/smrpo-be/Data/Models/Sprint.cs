using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace smrpo_be.Data.Models
{
    public class Sprint : Entity
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Velocity { get; set; }
        public Guid ProjectId { get; set; }

        [NotMapped]
        public bool Active 
        { 
            get { return Start <= DateTime.Now && End >= DateTime.Now; }
        }

        // Relations
        public virtual Project Project { get; set; }
        public virtual IEnumerable<UserStory> UserStories { get; set; }
        public virtual IEnumerable<UserStoryTime> UserStoryTimes { get; set; }
    }
}
