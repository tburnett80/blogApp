using System.Collections.Generic;

namespace BlogApp.Common.Models
{
    public class PostHeader
    {
        public PostHeader()
        {
            Tags = new Tag[0];
        }

        public int Id { get; set; }

        public int BodyId { get; set; }

        public string Title { get; set; }

        public IEnumerable<Tag> Tags { get; set; }
    }
}
