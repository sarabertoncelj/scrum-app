using smrpo_be.Data.Enums;
using System;
using System.Collections.Generic;

namespace smrpo_be.Data.Models
{
    public class User : Entity
    {
        public string Username { get; set; }
        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public UserRole Role { get; set; }

        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public DateTime LastLogin { get; set; }
        public DateTime NewLogin { get; set; }

        //Relations
        public virtual IEnumerable<UserProject> Projects { get; set; }
        public virtual IEnumerable<UserStoryTask> Tasks { get; set; }
        public virtual IEnumerable<ProjectPost> ProjectPosts { get; set; }
        public virtual IEnumerable<WorkLog> WorkLogs { get; set; }
    }
}
