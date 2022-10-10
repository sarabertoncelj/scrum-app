using smrpo_be.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace smrpo_be.Data.Models
{
    public class UserProjectRole : Entity
    {

        public ProjectRole Role { get; set; }

        public virtual UserProject UserProject { get; set; }
    }
}
