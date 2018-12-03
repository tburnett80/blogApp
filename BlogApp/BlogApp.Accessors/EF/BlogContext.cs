using BlogApp.Accessors.EF.mappings;
using BlogApp.Accessors.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Accessors.EF
{
    internal class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new TagEntityMapping());
            //modelBuilder.ApplyConfiguration(new PostBodyEntityMapping());
            //modelBuilder.ApplyConfiguration(new PostHeaderEntityMapping());
            //modelBuilder.ApplyConfiguration(new PostTagEntityMapping());
        }

        internal DbSet<TagEntity> Tags { get; set; }

        //internal DbSet<PostBodyEntity> Bodies { get; set; }

        //internal DbSet<PostHeaderEntity> Headers { get; set; }
    }
}
