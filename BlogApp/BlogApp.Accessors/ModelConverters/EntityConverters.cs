using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BlogApp.Accessors.Entities;
using BlogApp.Common.Models;

namespace BlogApp.Accessors.ModelConverters
{
    /// <summary>
    /// Extension methods to convert Entities to Models
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    internal static class EntityConverters
    {
        internal static IEnumerable<PostHeader> Convert(this ICollection<PostHeaderEntity> ents)
        {
            return ents == null 
                ? new PostHeader[0] 
                : ents.Select(e => e.Convert());
        }

        internal static PostHeader Convert(this PostHeaderEntity ent)
        {
            if (ent == null)
                return null;

            return new PostHeader
            {
                Id = ent.Id,
                BodyId = ent.BodyId,
                Title = ent.Title,
                Tags = ent.PostTags.Select(e => e.Tag).Convert()
            };
        }

        internal static IEnumerable<Tag> Convert(this IEnumerable<TagEntity> ents)
        {
            return ents == null 
                ? new Tag[0] 
                : ents.Select(e => e.Convert());
        }

        internal static Tag Convert(this TagEntity ent)
        {
            if (ent == null)
                return null;

            return new Tag
            {
                Id = ent.Id,
                Text = ent.Text
            };
        }

        internal static Post Convert(this PostBodyEntity ent, PostHeaderEntity ent2)
        {
            if (ent == null || ent2 == null)
                return null;

            return new Post
            {
                Header = ent2.Convert(),
                Body = ent.Markdown
            };
        }
    }
}
