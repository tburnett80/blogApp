using BlogApp.Accessors.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Accessors.EF.mappings
{
    internal sealed class PostTagEntityMapping : IEntityTypeConfiguration<PostTagEntity>
    {
        public void Configure(EntityTypeBuilder<PostTagEntity> builder)
        {
            builder.ToTable("post_tags")
                .HasKey(e => new { e.TagId, e.PostId });

            builder.Property(e => e.TagId)
                .HasColumnName("tag_id")
                .IsRequired();

            builder.Property(e => e.PostId)
                .HasColumnName("post_header_id")
                .IsRequired();

            builder.HasOne(e => e.Tag)
                .WithMany()
                .HasForeignKey(e => e.TagId);

            builder.HasOne(e => e.Post)
                .WithMany()
                .HasForeignKey(e => e.PostId);
        }
    }
}
