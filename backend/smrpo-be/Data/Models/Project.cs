using System.Collections.Generic;

namespace smrpo_be.Data.Models
{
    public class Project : Entity
    {
        public string Name { get; set; }

        //Relations
        public virtual IEnumerable<UserProject> Users { get; set; }
        public virtual IEnumerable<UserStory> UserStories { get; set; }
        public virtual IEnumerable<Sprint> Sprints { get; set; }
        public virtual IEnumerable<ProjectPost> ProjectPosts { get; set; }
    }
}
