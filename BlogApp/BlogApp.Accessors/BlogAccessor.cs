using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using BlogApp.Common.Contracts.Accessors;

[assembly: InternalsVisibleTo("BlogApp.Accessors.Tests")]
namespace BlogApp.Accessors
{
    public class BlogAccessor : IBlogAccessor
    {
    }
}
