﻿using Microsoft.EntityFrameworkCore;
using PostGram.DAL.Entities;
using PostGram.DAL.Entities.Configs;

namespace PostGram.DAL
{
    public class DataContext : DbContext
    {
        private const string MigrationsAssembly = "PostGram.Api";

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(ob => ob.MigrationsAssembly(MigrationsAssembly));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfig());
            modelBuilder.ApplyConfiguration(new AvatarConfig());
            modelBuilder.ApplyConfiguration(new PostContentConfig());
            modelBuilder.ApplyConfiguration(new LikeConfig());
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<UserSession> UserSessions => Set<UserSession>();
        public DbSet<Attachment> Attachments => Set<Attachment>();
        public DbSet<Avatar> Avatars => Set<Avatar>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<PostContent> PostContents => Set<PostContent>();
        public DbSet<Like> Likes => Set<Like>();
    }
}