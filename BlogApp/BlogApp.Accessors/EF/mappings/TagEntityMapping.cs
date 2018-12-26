using BlogApp.Accessors.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Accessors.EF.mappings
{
    internal sealed class TagEntityMapping : IEntityTypeConfiguration<TagEntity>
    {
        public void Configure(EntityTypeBuilder<TagEntity> builder)
        {
            builder.ToTable("tags")
                .HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.Text)
                .HasColumnName("tag_text")
                .HasMaxLength(64)
                .IsUnicode()
                .IsRequired();

            builder.HasIndex(e => e.Text)
                .HasName("IX_Tag_Name")
                .IsUnique();
        }
    }
}
