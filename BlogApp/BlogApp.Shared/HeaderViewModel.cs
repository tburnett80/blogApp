using System;
using System.Collections.Generic;
using System.Text;

namespace BlogApp.Shared
{
    public class HeaderViewModel
    {
        public HeaderViewModel()
        {
            Tags = new string[0];
        }

        public int Id { get; set; }

        public int BodyId { get; set; }

        public string Title { get; set; }

        public IEnumerable<string> Tags { get; set; }
    }
}
