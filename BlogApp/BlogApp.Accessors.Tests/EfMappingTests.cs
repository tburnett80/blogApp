﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BlogApp.Accessors.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlogApp.Accessors.Tests
{
    [TestClass]
    public class EfMappingTests
    {
        [TestMethod]
        [TestCategory("Integration Test")]
        public async Task TestBlogMappings()
        {
            var opts = new DbContextOptionsBuilder();
            opts.UseNpgsql("Server=10.200.7.50;Port=5432;Database=blog;User Id=user1;Password=password1;");

            using (var ctx = new BlogContext(opts.Options))
            {
                var tags = await ctx.Tags.ToListAsync();
                //var bodies = await ctx.Bodies.ToListAsync();
                //var headers = await ctx.Headers.ToListAsync();

                int x = 0;

            }
            
        }
    }
}