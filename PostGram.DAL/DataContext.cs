using Microsoft.EntityFrameworkCore;
using PostGram.DAL.Entities;
using PostGram.DAL.Entities.Configs;

namespace PostGram.DAL
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Attachment> Attachments => Set<Attachment>();

        public DbSet<Avatar> Avatars => Set<Avatar>();

        public DbSet<Comment> Comments => Set<Comment>();

        public DbSet<Like> Likes => Set<Like>();

        public DbSet<PostContent> PostContents => Set<PostContent>();

        public DbSet<Post> Posts => Set<Post>();

        public DbSet<Subscription> Subscriptions => Set<Subscription>();

        public DbSet<User> Users => Set<User>();

        public DbSet<UserSession> UserSessions => Set<UserSession>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfig());
            modelBuilder.ApplyConfiguration(new AvatarConfig());
            modelBuilder.ApplyConfiguration(new PostContentConfig());
            modelBuilder.ApplyConfiguration(new LikeConfig());
            modelBuilder.ApplyConfiguration(new SubscriptionConfig());
            modelBuilder.ApplyConfiguration(new PostConfig());
            modelBuilder.ApplyConfiguration(new CommentConfig());
            modelBuilder.ApplyConfiguration(new AttachmentConfig());
        }
    }
}