using AutoMapper;
using smrpo_be.Data;
using smrpo_be.Data.Models;
using smrpo_be.Data.WebModels;
using smrpo_be.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using smrpo_be.Data.Requests.User;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;

namespace smrpo_be.Services
{
    public interface IUserService
    {
        UserDto Authenticate(UserAuthentication model);
        IEnumerable<UserDto> GetAll();
        UserDto Get(Guid id);
        UserDto Create(UserRegistration model);
        void Update(UserUpdate model);
        IEnumerable<UserSearchableDto> Search(string username);
        void Delete(Guid id);
    }


    public class UserService : IUserService
    {
        private readonly SmrpoContext db;
        private readonly IMapper map;
        private readonly TokenValidation settings;
        private readonly IAuthenticationService _authService;

        public UserService(SmrpoContext context, IMapper mapper, IOptions<TokenValidation> appSettings, IAuthenticationService authService)
        {
            db = context;
            map = mapper;
            settings = appSettings.Value;
            _authService = authService;
        }

        public UserDto Create(UserRegistration model)
        {
            User currentUser = _authService.CurrentUser();
            if (currentUser.Role != Data.Enums.UserRole.Administrator) throw new AppException("Only system administrator can create users");

            if (string.IsNullOrWhiteSpace(model.Email)) throw new AppException("Email is required");
            if (string.IsNullOrWhiteSpace(model.Username)) throw new AppException("Username is required");
            if (string.IsNullOrWhiteSpace(model.Password)) throw new AppException("Password is required");
            if (!IsValidEmail(model.Email)) throw new AppException("Email is wrong format.");
            if (!IsStrongPassword(model.Password)) throw new AppException("Password requires at least 8 characters, " +
                "at least one UC letter, " +
                "at least one LC letter, " +
                "at least one non-letter char (digit OR special char)");
            if (db.Users.Any(x => x.Username == model.Username)) throw new AppException("Username is already taken");
            if (db.Users.Any(x => x.Email == model.Email)) throw new AppException("Email is already taken");

            User user = map.Map<User>(model);

            CreatePasswordHash(model.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            user.NewLogin = new DateTime(1970, 1, 1);

            db.Users.Add(user);
            db.SaveChanges();

            return map.Map<UserDto>(user);
        }

        public UserDto Authenticate(UserAuthentication model)
        {
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password)) return null;

            User user = db.Users.FirstOrDefault(x => x.Username.Equals(model.Username, StringComparison.InvariantCulture));
            if (user == null) return null;

            if (!VerifyPasswordHash(model.Password, user.PasswordHash, user.PasswordSalt)) return null;

            user.LastLogin = user.NewLogin;
            user.NewLogin = DateTime.Now;
            db.Update(user);
            db.SaveChanges();

            UserDto _user = map.Map<UserDto>(user);

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(settings.IssuerSigningKey);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Issuer = settings.ValidIssuer,
                Audience = settings.ValidAudience,
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            _user.Token = tokenHandler.WriteToken(token);

            return _user;
        }

        public IEnumerable<UserDto> GetAll()
        {
            IEnumerable<UserDto> users = map.Map<IEnumerable<UserDto>>(db.Users.AsEnumerable());
            return users;
        }

        public UserDto Get(Guid id)
        {
            User user = db.Users.Find(id);
            if (user == null) throw new AppException("User not found");

            return map.Map<UserDto>(user);
        }

        public void Update(UserUpdate model)
        {
            User user = db.Users.Find(model.Id);
            if (user == null) throw new AppException("User not found");

            if (!string.IsNullOrWhiteSpace(model.Email))
                user.Email = model.Email;

            if (!string.IsNullOrWhiteSpace(model.FirstName))
                user.FirstName = model.FirstName;

            if (!string.IsNullOrWhiteSpace(model.LastName))
                user.LastName = model.LastName;

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                CreatePasswordHash(model.Password, out byte[] passwordHash, out byte[] passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            db.Users.Update(user);
            db.SaveChanges();
        }

        public IEnumerable<UserSearchableDto> Search(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return new List<UserSearchableDto>();
            }

            username = username.TrimStart();
            IEnumerable<User> users = db.Users
                .Where(x => x.Username.StartsWith(username, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => new User { Id = x.Id, Username = x.Username });
            return map.Map<IEnumerable<UserSearchableDto>>(users);
        }

        public void Delete(Guid id)
        {
            User user = db.Users.Find(id);
            if (user == null) throw new AppException("User not found");

            db.Users.Remove(user);
            db.SaveChanges();
        }

        #region helpers

        private static bool IsValidEmail(string email)
        {
            // source: http://thedailywtf.com/Articles/Validating_Email_Addresses.aspx
            Regex rx = new Regex(
            @"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$");
            return rx.IsMatch(email);
        }

        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (HMACSHA512 hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (HMACSHA512 hmac = new HMACSHA512(storedSalt))
            {
                byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }


        /// <summary>
        /// - minimum 8 characters
        /// - at lease one UC letter
        /// - at least one LC letter
        /// - at least one non-letter char (digit OR special char)
        /// </summary>
        public static bool IsStrongPassword(string password)
        {
            return HasMinimumLength(password, 8)
                && HasUpperCaseLetter(password)
                && HasLowerCaseLetter(password)
                && (HasDigit(password) || HasSpecialChar(password));
        }

        public static bool HasMinimumLength(string password, int minLength)
        {
            return password.Length >= minLength;
        }

        public static bool HasMinimumUniqueChars(string password, int minUniqueChars)
        {
            return password.Distinct().Count() >= minUniqueChars;
        }

        public static bool HasDigit(string password)
        {
            return password.Any(c => char.IsDigit(c));
        }


        public static bool HasSpecialChar(string password)
        {
            return password.IndexOfAny("!@#$%^&*?_~-£().,".ToCharArray()) != -1;
        }

        public static bool HasUpperCaseLetter(string password)
        {
            return password.Any(c => char.IsUpper(c));
        }


        public static bool HasLowerCaseLetter(string password)
        {
            return password.Any(c => char.IsLower(c));
        }
    }


    #endregion
}

