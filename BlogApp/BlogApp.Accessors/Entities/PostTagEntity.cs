
namespace BlogApp.Accessors.Entities
{
    internal class PostTagEntity
    {
        public int TagId { get; set; }

        public int PostId { get; set; }

        public virtual TagEntity Tag { get; set; }

        public virtual PostHeaderEntity Post { get; set; }
    }
}
