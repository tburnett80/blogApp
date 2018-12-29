using System.Collections.Generic;
using System.Linq;
using BlogApp.Common.Models;
using BlogApp.Shared;

namespace BlogApp.Server.ModelMappers
{
    public static class ToViewModel
    {
        public static IEnumerable<MetaTagViewModel> Convert(this IEnumerable<MetaTag> dtos)
        {
            return dtos.Select(d => d.Convert()).ToList();
        }

        public static IEnumerable<HeaderViewModel> Convert(this IEnumerable<PostHeader> dtos)
        {
            return dtos.Select(d => d.Convert()).ToList();
        }

        public static HeaderViewModel Convert(this PostHeader dto)
        {
            if (dto == null)
                return null;

            return new HeaderViewModel
            {
                BodyId = dto.BodyId,
                Id = dto.Id,
                Title = dto.Title,
                Tags = dto.Tags.Select(t => t.Text)
            };
        }

        public static MetaTagViewModel Convert(this MetaTag dto)
        {
            if (dto == null)
                return null;

            return new MetaTagViewModel
            {
                Count = dto.Count,
                Tag = dto.Tag
            };
        }
    }
}
