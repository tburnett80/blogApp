using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace BlogApp.Accessors.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    internal class PostHeaderEntity
    {
        public int Id { get; set; }

        public int BodyId { get; set; }

        public string Title { get; set; }

        public virtual PostBodyEntity Body { get; set; }
    }
}
