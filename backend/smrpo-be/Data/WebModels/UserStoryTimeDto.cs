using smrpo_be.Data.Enums;
using System.Collections.Generic;
using System;

namespace smrpo_be.Data.WebModels
{
    public class UserStoryTimeDto
    {
        public int Estimation { get; set; }

        public SprintMetaDto Sprint { get; set; }
    }
}
