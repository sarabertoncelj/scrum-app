using System;

namespace smrpo_be.Data.WebModels
{
    public class ProjectPostDto : EntityDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateModified { get; set; }
        public UserDto User { get; set; }
    }
}
