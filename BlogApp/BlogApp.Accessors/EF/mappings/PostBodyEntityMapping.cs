using BlogApp.Accessors.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Accessors.EF.mappings
{
    internal sealed class PostBodyEntityMapping : IEntityTypeConfiguration<PostBodyEntity>
    {
        public void Configure(EntityTypeBuilder<PostBodyEntity> builder)
        {
            builder.ToTable("post_body")
                .HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.Markdown)
                .HasColumnName("post_markdown")
                .IsUnicode()
                .IsRequired();
        }
    }
}
