using smrpo_be.Data.Enums;
using System.Collections.Generic;
using System;

namespace smrpo_be.Data.WebModels
{
    public class UserStoryDto : EntityDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public UserStoryPriority Priority { get; set; }
        public int BusinessValue { get; set; }
        public string Comment { get; set; }
        public IEnumerable<string> AcceptanceTests { get; set; }
        public UserStoryStatus Status { get; set; }
        public SprintMetaDto Sprint { get; set; }
    }
}
