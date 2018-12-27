using BlogApp.Accessors.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Accessors.EF.mappings
{
    internal class MetaTagEntityMapping : IEntityTypeConfiguration<MetaTagEntity>
    {
        public void Configure(EntityTypeBuilder<MetaTagEntity> builder)
        {
            builder.HasKey(e => new { e.Tag, e.Count });

            builder.Property(e => e.Tag)
                .HasMaxLength(64)
                .HasColumnName("tag");

            builder.Property(e => e.Count)
                .HasColumnName("count");
        }
    }
}
