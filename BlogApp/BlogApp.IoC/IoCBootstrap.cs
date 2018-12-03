using System;
using System.Collections.Generic;
using System.Text;
using BlogApp.Accessors;
using BlogApp.Common.Contracts.Accessors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.IoC
{
    public static class IoCBootstrap
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Config options
            var ctxConn = new DbContextOptionsBuilder();
            ctxConn.UseNpgsql(configuration["DbConnStr"]);
            //ctxConn.UseSqlServer(configuration["ConnStr"]);

            //TODO: shut off before v1
            ctxConn.EnableSensitiveDataLogging();
            services.AddSingleton(ctxConn.Options);

            //Accessors
            services.AddScoped<IBlogAccessor, BlogAccessor>();

            //TODO: Add engines

            //TODO: Add managers
        }
    }
}
