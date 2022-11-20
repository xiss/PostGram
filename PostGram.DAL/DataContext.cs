using Microsoft.EntityFrameworkCore;
using PostGram.DAL.Entities;
using PostGram.DAL.Entities.Configs;
using PostGram.Common.Constants;

namespace PostGram.DAL
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(ob => ob.MigrationsAssembly(Ef.MigrationsAssembly));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfig());
            modelBuilder.ApplyConfiguration(new AvatarConfig());
            modelBuilder.ApplyConfiguration(new PostContentConfig());
            modelBuilder.ApplyConfiguration(new LikeConfig());
            modelBuilder.ApplyConfiguration(new SubscriptionConfig());
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<UserSession> UserSessions => Set<UserSession>();
        public DbSet<Attachment> Attachments => Set<Attachment>();
        public DbSet<Avatar> Avatars => Set<Avatar>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<PostContent> PostContents => Set<PostContent>();
        public DbSet<Like> Likes => Set<Like>();
        public DbSet<Subscription> Subscriptions => Set<Subscription>();
    }
}