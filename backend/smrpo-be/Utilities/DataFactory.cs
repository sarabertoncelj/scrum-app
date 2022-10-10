using smrpo_be.Data;
using smrpo_be.Data.Models;
using smrpo_be.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace smrpo_be.Utilities
{
    public class DataFactory
    {
        private readonly SmrpoContext db;

        public DataFactory(SmrpoContext context)
        {
            db = context;
            CreateUsers();
            CreateProjects();
        }

        public void CreateUsers()
        {
            User admin = new User()
            {
                Username = "admin",
                Email = "admin@smrpo.com",
                Role = Data.Enums.UserRole.Administrator,
                FirstName = "Root",
                LastName = "Admin",
            };

            SetPassword(admin, "admin");

            if (!db.Users.Where(x => x.Username == admin.Username).Any())
            {
                db.Add(admin);
                db.SaveChanges();
            }


            User user = new User()
            {
                Username = "user",
                Email = "user@smrpo.com",
                Role = Data.Enums.UserRole.User,
                FirstName = "Normal",
                LastName = "User",
            };

            SetPassword(user, "user");

            if (!db.Users.Where(x => x.Username == user.Username).Any())
            {
                db.Add(user);
                db.SaveChanges();
            }
        }


        public void CreateProjects()
        {
            User po = new User()
            {
                Username = "seedPO",
                Email = "seedPO@smrpo.com",
                Role = Data.Enums.UserRole.User,
                FirstName = "Seed",
                LastName = "ProjectOwner",
            };
            SetPassword(po, "seed");

            User sm = new User()
            {
                Username = "seedSM",
                Email = "seedSM@smrpo.com",
                Role = Data.Enums.UserRole.User,
                FirstName = "Seed",
                LastName = "ScrumMaster",
            };
            SetPassword(sm, "seed");

            User m = new User()
            {
                Username = "seedM",
                Email = "seedM@smrpo.com",
                Role = Data.Enums.UserRole.User,
                FirstName = "Seed",
                LastName = "Member",
            };
            SetPassword(m, "seed");

            if (!db.Users.Where(x => x.Username == po.Username).Any()) db.Add(po);
            if (!db.Users.Where(x => x.Username == sm.Username).Any()) db.Add(sm);
            if (!db.Users.Where(x => x.Username == m.Username).Any()) db.Add(m);
            db.SaveChanges();

            UserStory us1 = new UserStory()
            {
                Name = "#1 Test user story",
                Description = "This is test description of user story",
                Priority = Data.Enums.UserStoryPriority.MustHave,
                BusinessValue = 10,
                AcceptanceTests = new List<AcceptanceTest>()
                {
                    new AcceptanceTest() { Description = "Acceptance test #1" },
                    new AcceptanceTest() { Description = "Acceptance test #2" },
                    new AcceptanceTest() { Description = "Acceptance test #3" },
                },
                Tasks = new List<UserStoryTask>()
                {
                    new UserStoryTask() { Description = "Task #1", RemainingTime = 10, User = null, Status = Data.Enums.UserStoryTaskStatus.Unassigned },
                    new UserStoryTask() { 
                        Description = "Task #2", RemainingTime = 5, ActiveFrom = DateTime.Now, Status = Data.Enums.UserStoryTaskStatus.Assigned, Accepted = true, User = m,
                        WorkLogs = new List<WorkLog>
                        {
                            new WorkLog
                            {
                                User = m,
                                HoursWorked = 1,
                                HoursRemaining = 6,
                                Day = DateTime.Now.AddDays(-1)
                            },
                            new WorkLog {
                                User = m,
                                HoursWorked = 0,
                                HoursRemaining = 5,
                                Day = DateTime.Now
                            }
                        }
                    },
                    new UserStoryTask() { Description = "Task #3", RemainingTime = 0, Status = Data.Enums.UserStoryTaskStatus.Finished, Accepted = true, User = m },
                    new UserStoryTask() { Description = "Task #4", RemainingTime = 24, User = sm, Status = Data.Enums.UserStoryTaskStatus.Assigned, Accepted = false},
                    new UserStoryTask() { Description = "Task #5", RemainingTime = 24, User = sm, Status = Data.Enums.UserStoryTaskStatus.Assigned, Accepted = true},
                    new UserStoryTask() { Description = "Task #6", RemainingTime = 10, ActiveFrom = DateTime.Now, User = sm, Status = Data.Enums.UserStoryTaskStatus.InProgress, Accepted = true },
                }
            };

            UserStory us2 = new UserStory()
            {
                Name = "#2 Test user story",
                Description = "This is test description of user story #2",
                Priority = Data.Enums.UserStoryPriority.ShouldHave,
                BusinessValue = 6,
                AcceptanceTests = new List<AcceptanceTest>()
                {
                    new AcceptanceTest() { Description = "Acceptance test #2.1" },
                    new AcceptanceTest() { Description = "Acceptance test #2.2" },
                    new AcceptanceTest() { Description = "Acceptance test #2.3" },
                },
                Tasks = new List<UserStoryTask>()
                {
                    new UserStoryTask() { Description = "Task #1", RemainingTime = 0, User = sm, Status = Data.Enums.UserStoryTaskStatus.Finished, Accepted = true }
                }
            };

            UserStory us3 = new UserStory()
            {
                Name = "#3 Test user story",
                Description = "This is test description of user story #3 which is finished",
                Priority = Data.Enums.UserStoryPriority.CouldHave,
                BusinessValue = 4,
                Status = Data.Enums.UserStoryStatus.Finished,
                AcceptanceTests = new List<AcceptanceTest>()
                {
                    new AcceptanceTest() { Description = "Acceptance test #3.1" },
                    new AcceptanceTest() { Description = "Acceptance test #3.2" },
                    new AcceptanceTest() { Description = "Acceptance test #3.3" },
                },
            };

            UserStory us4 = new UserStory()
            {
                Name = "#4 Test user story",
                Description = "This is test description of user story #4",
                Priority = Data.Enums.UserStoryPriority.WontHaveThisTime,
                BusinessValue = 1,
                AcceptanceTests = new List<AcceptanceTest>()
                {
                    new AcceptanceTest() { Description = "Acceptance test #4.1" },
                    new AcceptanceTest() { Description = "Acceptance test #4.2" },
                    new AcceptanceTest() { Description = "Acceptance test #4.3" },
                },
            };

            Sprint s1 = null;
            s1 = new Sprint()
            {
                Start = DateTime.Now.AddDays(-2),
                End = DateTime.Now.AddDays(7),
                Velocity = 10,
                UserStories = new List<UserStory>() {
                    us1,
                    us2
                },
                UserStoryTimes = new List<UserStoryTime>()
                {
                    new UserStoryTime() { UserStory = us1, Sprint = s1, Estimation = 5 },
                    new UserStoryTime() { UserStory = us2, Sprint = s1, Estimation = 2},
                    new UserStoryTime() { UserStory = us3, Sprint = s1 },
                    new UserStoryTime() { UserStory = us4, Sprint = s1 }
                }
            };

            Sprint s2 = null;
            s2 = new Sprint()
            {
                Start = DateTime.Now.AddDays(8),
                End = DateTime.Now.AddDays(15),
                Velocity = 10,
                UserStories = new List<UserStory>() {
                },
                UserStoryTimes = new List<UserStoryTime>()
                {
                    new UserStoryTime() { UserStory = us1, Sprint = s2 },
                    new UserStoryTime() { UserStory = us2, Sprint = s2 },
                    new UserStoryTime() { UserStory = us3, Sprint = s2 },
                    new UserStoryTime() { UserStory = us4, Sprint = s2 }
                }
            };

            Project p1 = new Project()
            {
                Name = "Seed Project",
                Users = new List<UserProject>()
                {
                    new UserProject() { User = db.Users.First(x => x.Username == "seedPO"), ProjectRoles = new List<UserProjectRole>() { new UserProjectRole() {Role = Data.Enums.ProjectRole.ProductOwner } } },
                    new UserProject() { User = db.Users.First(x => x.Username == "seedSM"), ProjectRoles = new List<UserProjectRole>() {
                        new UserProjectRole() {Role = Data.Enums.ProjectRole.ScrumMaster },
                        new UserProjectRole() {Role = Data.Enums.ProjectRole.Developer }
                    } },
                    new UserProject() { User = db.Users.First(x => x.Username == "seedM"), ProjectRoles = new List<UserProjectRole>() { new UserProjectRole() {Role = Data.Enums.ProjectRole.Developer } } },
                },
                UserStories = new List<UserStory>() { us1, us2, us3, us4 },
                Sprints = new List<Sprint>() { s1, s2 },
                ProjectPosts = new List<ProjectPost>()
                {
                    new ProjectPost { Title = "Test project post #1", Description = "Test project post #1 content", User = db.Users.First(x => x.Username == "seedSM") },
                    new ProjectPost { Title = "Test project post #2", Description = "Test project post #2 content", User = db.Users.First(x => x.Username == "seedM") }
                }
            };

            if (!db.Projects.Where(x => x.Name == p1.Name).Any())
            {
                db.Add(p1);
                db.SaveChanges();
            };
        }

        #region helpers

        private void SetPassword(User user, string password)
        {
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (HMACSHA512 hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
        #endregion
    }
}
