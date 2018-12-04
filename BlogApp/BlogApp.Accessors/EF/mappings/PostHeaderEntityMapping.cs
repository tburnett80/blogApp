using BlogApp.Accessors.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Accessors.EF.mappings
{
    internal sealed class PostHeaderEntityMapping : IEntityTypeConfiguration<PostHeaderEntity>
    {
        public void Configure(EntityTypeBuilder<PostHeaderEntity> builder)
        {
            builder.ToTable("post_header")
                .HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.BodyId)
                .HasColumnName("post_body_id")
                .IsRequired();

            builder.Property(e => e.Title)
                .HasColumnName("post_title")
                .HasMaxLength(254)
                .IsUnicode()
                .IsRequired();

            builder.HasOne(e => e.Body)
                .WithMany()
                .HasForeignKey(e => e.BodyId);

            
        }
    }
}
