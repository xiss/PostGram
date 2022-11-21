﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PostGram.DAL;

#nullable disable

namespace PostGram.Api.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PostGram.DAL.Entities.Attachment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("MimeType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("Size")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Attachments");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("PostGram.DAL.Entities.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset?>("Edited")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("QuotedCommentId")
                        .HasColumnType("uuid");

                    b.Property<string>("QuotedText")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("PostId");

                    b.HasIndex("QuotedCommentId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("PostGram.DAL.Entities.Like", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("CommentId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("EntityId")
                        .HasColumnType("uuid");

                    b.Property<int>("EntityType")
                        .HasColumnType("integer");

                    b.Property<bool?>("IsLike")
                        .HasColumnType("boolean");

                    b.Property<Guid?>("PostId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId")
                        .IsUnique();

                    b.HasIndex("CommentId");

                    b.HasIndex("PostId");

                    b.HasIndex("AuthorId", "EntityId")
                        .IsUnique();

                    b.ToTable("Likes");
                });

            modelBuilder.Entity("PostGram.DAL.Entities.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset?>("Edited")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Header")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("PostGram.DAL.Entities.Subscription", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset?>("Edited")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("MasterId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("SlaveId")
                        .HasColumnType("uuid");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("SlaveId");

                    b.HasIndex("MasterId", "SlaveId")
                        .IsUnique();

                    b.ToTable("Subscriptions");
                });

            modelBuilder.Entity("PostGram.DAL.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("AvatarId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("BirthDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsPrivate")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Nickname")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Patronymic")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AvatarId")
                        .IsUnique();

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PostGram.DAL.Entities.UserSession", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<Guid>("RefreshTokenId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserSessions");
                });

            modelBuilder.Entity("PostGram.DAL.Entities.Avatar", b =>
                {
                    b.HasBaseType("PostGram.DAL.Entities.Attachment");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Avatars", (string)null);
                });

            modelBuilder.Entity("PostGram.DAL.Entities.PostContent", b =>
                {
                    b.HasBaseType("PostGram.DAL.Entities.Attachment");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uuid");

                    b.HasIndex("PostId");

                    b.ToTable("PostContents", (string)null);
                });

            modelBuilder.Entity("PostGram.DAL.Entities.Attachment", b =>
                {
                    b.HasOne("PostGram.DAL.Entities.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId");

                    b.Navigation("Author");
                });

            modelBuilder.Entity("PostGram.DAL.Entities.Comment", b =>
                {
                    b.HasOne("PostGram.DAL.Entities.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PostGram.DAL.Entities.Post", "Post")
                        .WithMany("Comments")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PostGram.DAL.Entities.Comment", "QuotedComment")
                        .WithMany()
                        .HasForeignKey("QuotedCommentId");

                    b.Navigation("Author");

                    b.Navigation("Post");

                    b.Navigation("QuotedComment");
                });

            modelBuilder.Entity("PostGram.DAL.Entities.Like", b =>
                {
                    b.HasOne("PostGram.DAL.Entities.User", "Author")
                        .WithOne()
                        .HasForeignKey("PostGram.DAL.Entities.Like", "AuthorId");

                    b.HasOne("PostGram.DAL.Entities.Comment", null)
                        .WithMany("Likes")
                        .HasForeignKey("CommentId");

                    b.HasOne("PostGram.DAL.Entities.Post", null)
                        .WithMany("Likes")
                        .HasForeignKey("PostId");

                    b.Navigation("Author");
                });

            modelBuilder.Entity("PostGram.DAL.Entities.Post", b =>
                {
                    b.HasOne("PostGram.DAL.Entities.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("PostGram.DAL.Entities.Subscription", b =>
                {
                    b.HasOne("PostGram.DAL.Entities.User", "Master")
                        .WithMany("Masters")
                        .HasForeignKey("MasterId");

                    b.HasOne("PostGram.DAL.Entities.User", "Slave")
                        .WithMany("Slaves")
                        .HasForeignKey("SlaveId");

                    b.Navigation("Master");

                    b.Navigation("Slave");
                });

            modelBuilder.Entity("PostGram.DAL.Entities.User", b =>
                {
                    b.HasOne("PostGram.DAL.Entities.Avatar", "Avatar")
                        .WithOne("User")
                        .HasForeignKey("PostGram.DAL.Entities.User", "AvatarId");

                    b.Navigation("Avatar");
                });

            modelBuilder.Entity("PostGram.DAL.Entities.UserSession", b =>
                {
                    b.HasOne("PostGram.DAL.Entities.User", "User")
                        .WithMany("Sessions")
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PostGram.DAL.Entities.Avatar", b =>
                {
                    b.HasOne("PostGram.DAL.Entities.Attachment", null)
                        .WithOne()
                        .HasForeignKey("PostGram.DAL.Entities.Avatar", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PostGram.DAL.Entities.PostContent", b =>
                {
                    b.HasOne("PostGram.DAL.Entities.Attachment", null)
                        .WithOne()
                        .HasForeignKey("PostGram.DAL.Entities.PostContent", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PostGram.DAL.Entities.Post", "Post")
                        .WithMany("PostContents")
                        .HasForeignKey("PostId");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("PostGram.DAL.Entities.Comment", b =>
                {
                    b.Navigation("Likes");
                });

            modelBuilder.Entity("PostGram.DAL.Entities.Post", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Likes");

                    b.Navigation("PostContents");
                });

            modelBuilder.Entity("PostGram.DAL.Entities.User", b =>
                {
                    b.Navigation("Masters");

                    b.Navigation("Sessions");

                    b.Navigation("Slaves");
                });

            modelBuilder.Entity("PostGram.DAL.Entities.Avatar", b =>
                {
                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
