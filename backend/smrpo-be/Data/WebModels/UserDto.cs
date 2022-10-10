using smrpo_be.Data.Enums;
using System;

namespace smrpo_be.Data.WebModels
{
    public class UserDto : EntityDto
    {
        public string Username { get; set; }
        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public UserRole Role { get; set; }

        public string Token { get; set; }
        public DateTime LastLogin { get; set; }
    }

    public class UserSearchableDto : EntityDto
    {
        public string Username { get; set; }
    }
}
