using smrpo_be.Data.Enums;

namespace smrpo_be.Data.Requests.User
{
    public class UserRegistration
    {
        public string Username { get; set; }
        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public UserRole Role { get; set; }

        public string Password { get; set; }
    }
}
