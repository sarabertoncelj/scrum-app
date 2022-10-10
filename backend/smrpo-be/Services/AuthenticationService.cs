using Microsoft.AspNetCore.Http;
using smrpo_be.Data;
using smrpo_be.Data.Models;
using smrpo_be.Utilities;
using System;
using System.Security.Claims;

namespace smrpo_be.Services
{
    public interface IAuthenticationService
    {
        User CurrentUser();
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly IHttpContextAccessor httpContext;
        private readonly SmrpoContext db;

        public AuthenticationService(IHttpContextAccessor httpContextAccessor, SmrpoContext context)
        {
            httpContext = httpContextAccessor;
            db = context;
        }

        public User CurrentUser()
        {
            try
            {
                string userId = httpContext.HttpContext.User.FindFirst(ClaimTypes.Name).Value;

                if (string.IsNullOrWhiteSpace(userId)) throw new AppException("Unauthorized", new { Code = 401 });

                Guid id = Guid.Parse(userId);
                User user = db.Users.Find(id);
                if (user == null) throw new AppException("Unauthorized", new { Code = 401 });
                return user;
            } 
            catch (Exception)
            {
                throw new AppException("Unauthorized", new { Code = 401 });
            }
        }
    }
}
