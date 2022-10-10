﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using smrpo_be.Data;

namespace smrpo_be.Migrations
{
    [DbContext(typeof(SmrpoContext))]
    [Migration("20200412160052_UserStoryTime")]
    partial class UserStoryTime
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("smrpo_be.Data.Models.AcceptanceTest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<Guid>("UserStoryId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("UserStoryId");

                    b.ToTable("AcceptanceTests");
                });

            modelBuilder.Entity("smrpo_be.Data.Models.Project", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("smrpo_be.Data.Models.Sprint", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("End")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("ProjectId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("Start")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Velocity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("Sprints");
                });

            modelBuilder.Entity("smrpo_be.Data.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("FirstName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("LastLogin")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("LastName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("smrpo_be.Data.Models.UserProject", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("char(36)");

                    b.Property<int>("ProjectRole")
                        .HasColumnType("int");

                    b.HasKey("UserId", "ProjectId");

                    b.HasIndex("ProjectId");

                    b.ToTable("UserProjects");
                });

            modelBuilder.Entity("smrpo_be.Data.Models.UserStory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("BusinessValue")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("SprintId")
                        .HasColumnType("char(36)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.HasIndex("SprintId");

                    b.ToTable("UserStories");
                });

            modelBuilder.Entity("smrpo_be.Data.Models.UserStoryTime", b =>
                {
                    b.Property<Guid>("UserStoryId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("SprintId")
                        .HasColumnType("char(36)");

                    b.Property<int>("Estimation")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.HasKey("UserStoryId", "SprintId");

                    b.HasIndex("SprintId");

                    b.ToTable("UserStoryTimes");
                });

            modelBuilder.Entity("smrpo_be.Data.Models.AcceptanceTest", b =>
                {
                    b.HasOne("smrpo_be.Data.Models.UserStory", "UserStory")
                        .WithMany("AcceptanceTests")
                        .HasForeignKey("UserStoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("smrpo_be.Data.Models.Sprint", b =>
                {
                    b.HasOne("smrpo_be.Data.Models.Project", "Project")
                        .WithMany("Sprints")
                        .HasForeignKey("ProjectId");
                });

            modelBuilder.Entity("smrpo_be.Data.Models.UserProject", b =>
                {
                    b.HasOne("smrpo_be.Data.Models.Project", "Project")
                        .WithMany("Users")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("smrpo_be.Data.Models.User", "User")
                        .WithMany("Projects")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("smrpo_be.Data.Models.UserStory", b =>
                {
                    b.HasOne("smrpo_be.Data.Models.Project", "Project")
                        .WithMany("UserStories")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("smrpo_be.Data.Models.Sprint", "Sprint")
                        .WithMany("UserStories")
                        .HasForeignKey("SprintId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("smrpo_be.Data.Models.UserStoryTime", b =>
                {
                    b.HasOne("smrpo_be.Data.Models.Sprint", "Sprint")
                        .WithMany("UserStoryTimes")
                        .HasForeignKey("SprintId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("smrpo_be.Data.Models.UserStory", "UserStory")
                        .WithMany("UserStoryTimes")
                        .HasForeignKey("UserStoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
