using smrpo_be.Data.Enums;
using System.Collections.Generic;
using System;

namespace smrpo_be.Data.Requests.UserStory
{
    public class UserStoryUpdate
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public UserStoryPriority Priority { get; set; }
        public int BusinessValue { get; set; }
        public IEnumerable<string> AcceptanceTests { get; set; }
    }
}
