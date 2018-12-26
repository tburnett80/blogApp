using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlogApp.Accessors.Entities;
using BlogApp.Common.Models;

namespace BlogApp.Accessors.ModelConverters
{
    internal static class DtoConverters
    {
        internal static PostHeaderEntity Convert(this Post dto)
        {
            var ent = dto.Header.Convert();
            ent.Body = new PostBodyEntity
            {
                Markdown = dto.Body
            };

            return ent;
        }

        internal static PostHeaderEntity Convert(this PostHeader dto)
        {
            if (dto == null)
                return null;

            return new PostHeaderEntity
            {
                Id = dto.Id,
                Title = dto.Title
            };
        }

        internal static IEnumerable<TagEntity> Convert(this IEnumerable<Tag> dtos)
        {
            return dtos.Select(d => d.Convert());
        }

        internal static TagEntity Convert(this Tag dto)
        {
            if (dto == null)
                return null;

            return new TagEntity
            {
                Id = dto.Id,
                Text = dto.Text
            };
        }

        internal static IEnumerable<PostTagEntity> Convert(this PostHeaderEntity ent, IEnumerable<Tag> ents)
        {
            return ents.Select(t => new PostTagEntity
            {
                PostId = ent.Id,
                TagId = t.Id
            });
        }
    }
}
