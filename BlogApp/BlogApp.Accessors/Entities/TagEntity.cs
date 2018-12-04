using System.Collections.Generic;

namespace BlogApp.Accessors.Entities
{
    internal class TagEntity
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public virtual ICollection<PostTagEntity> PostTags { get; set; }
    }
}
