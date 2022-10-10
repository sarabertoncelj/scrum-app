using smrpo_be.Data.Enums;
using System.Collections.Generic;
using System;

namespace smrpo_be.Data.Models
{
    public class ProjectPost : Entity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; }
		public Guid ProjectId { get; set; }

        //Relations
        public virtual Project Project { get; set; }
        public virtual User User { get; set; }
    }
}
