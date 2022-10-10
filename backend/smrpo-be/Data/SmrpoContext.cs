using smrpo_be.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using smrpo_be.Data.Enums;

namespace smrpo_be.Data
{
    public class SmrpoContext : DbContext
    {
        public SmrpoContext() { }

        public SmrpoContext(DbContextOptions<SmrpoContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectPost> ProjectPosts { get; set; }
        public DbSet<UserProject> UserProjects { get; set; }
        public DbSet<UserStory> UserStories { get; set; }
        public DbSet<AcceptanceTest> AcceptanceTests { get; set; }
        public DbSet<UserStoryTime> UserStoryTimes { get; set; }
        public DbSet<Sprint> Sprints { get; set; }
        public DbSet<UserStoryTask> Tasks { get; set; }
        public DbSet<WorkLog> WorkLogs { get; set; }
        public DbSet<UserProjectRole> UserProjectRole { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(pk => pk.Id);

                e.HasIndex(p => p.Username).IsUnique();
                e.HasIndex(p => p.Email).IsUnique();

                e.Property(p => p.Username).IsRequired();
                e.Property(p => p.Email).IsRequired();
                e.Property(p => p.Role).IsRequired();
                e.Property(p => p.PasswordHash).IsRequired();
                e.Property(p => p.PasswordSalt).IsRequired();
                e.Property(p => p.LastLogin);
                e.Property(p => p.NewLogin);

                e.HasMany(x => x.Projects)
                 .WithOne(x => x.User)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasMany(x => x.Tasks)
                 .WithOne(x => x.User)
                 .HasForeignKey(x => x.UserId);

                e.HasMany(x => x.ProjectPosts)
                 .WithOne(x => x.User)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasMany(x => x.WorkLogs)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Project>(e =>
            {
                e.HasKey(pk => pk.Id);

                e.HasIndex(p => p.Name).IsUnique();
                e.Property(p => p.Name).IsRequired();

                e.HasMany(x => x.Users)
                 .WithOne(x => x.Project)
                 .HasForeignKey(x => x.ProjectId)
                 .OnDelete(DeleteBehavior.Cascade);
                e.HasMany(x => x.UserStories)
                 .WithOne(x => x.Project)
                 .HasForeignKey(x => x.ProjectId)
                 .OnDelete(DeleteBehavior.Cascade);
                e.HasMany(x => x.ProjectPosts)
                 .WithOne(x => x.Project)
                 .HasForeignKey(x => x.ProjectId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ProjectPost>(e =>
            {
                e.HasKey(pk => pk.Id);

                e.Property(p => p.Title).IsRequired();
                e.Property(p => p.Description);
            });

            modelBuilder.Entity<UserProject>(e =>
            {
                e.HasKey(ck => new { ck.UserId, ck.ProjectId });

                e.HasMany(x => x.ProjectRoles)
                 .WithOne(x => x.UserProject)
                 .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<UserStory>(e =>
            {
                e.HasKey(pk => pk.Id);

                e.HasIndex(p => p.ProjectId);
                
                e.Property(p => p.Name).IsRequired();
                e.Property(p => p.Description);
                e.Property(p => p.Priority).IsRequired();
                e.Property(p => p.BusinessValue);
                e.Property(p => p.Deleted).HasDefaultValue(false);

                e.Property(p => p.ProjectId).IsRequired();
                e.Property(p => p.SprintId);
                e.HasMany(x => x.AcceptanceTests)
                 .WithOne(x => x.UserStory)
                 .HasForeignKey(x => x.UserStoryId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasMany(x => x.Tasks)
                 .WithOne(x => x.UserStory)
                 .HasForeignKey(x => x.UserStoryId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AcceptanceTest>(e =>
            {
                e.HasKey(pk => pk.Id);

                e.HasIndex(p => p.UserStoryId);

                e.Property(p => p.Description).IsRequired();
                e.Property(p => p.UserStoryId).IsRequired();
            });

            modelBuilder.Entity<UserStoryTime>(e =>
            {
                e.HasKey(ck => new { ck.UserStoryId, ck.SprintId });

                e.Property(p => p.Estimation).HasDefaultValue(0);
            });

            modelBuilder.Entity<Sprint>(e =>
            {
                e.HasKey(pk => pk.Id);

                e.Property(p => p.Start).IsRequired();
                e.Property(p => p.End).IsRequired();
                e.Property(p => p.Velocity).IsRequired();

                e.HasMany(x => x.UserStories)
                 .WithOne(x => x.Sprint)
                 .HasForeignKey(x => x.SprintId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<UserStoryTask>(e =>
            {
                e.HasKey(pk => pk.Id);

                e.Property(p => p.Description).IsRequired();
                e.Property(p => p.RemainingTime).IsRequired();
                e.Property(p => p.ActiveFrom);
                e.Property(p => p.Accepted);
                e.Property(p => p.Status);

                e.HasMany(x => x.WorkLogs)
                .WithOne(x => x.Task)
                .HasForeignKey(x => x.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<WorkLog>(e =>
            {
                e.HasKey(pk => pk.Id);

                e.Property(p => p.HoursWorked);
                e.Property(p => p.HoursRemaining);
                e.Property(p => p.Day).HasColumnType("date");
            });
        }

        #region Extensions

        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is Entity && (
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((Entity)entityEntry.Entity).DateModified = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                {
                    ((Entity)entityEntry.Entity).DateCreated = DateTime.Now;
                }
            }

            return base.SaveChanges();
        }

        #endregion
    }
}
