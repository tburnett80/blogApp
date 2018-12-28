using BlogApp.Accessors;
using BlogApp.Common.Contracts.Accessors;
using BlogApp.Common.Contracts.Engines;
using BlogApp.Common.Contracts.Managers;
using BlogApp.Engines;
using BlogApp.Managers;
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

            if(bool.Parse(configuration["SensDbLogging"]))
                ctxConn.EnableSensitiveDataLogging();

            services.AddSingleton(ctxConn.Options);
            services.AddSingleton<IConfiguration>(configuration);

            //Accessors
            services.AddSingleton<ICacheAccessor, BlogCacheAccessor>();
            services.AddScoped<IBlogAccessor, BlogAccessor>();
            services.AddScoped<IBlobAccessor, BlogBlobAccessor>();

            //Engines
            services.AddScoped<IBlogEngine, BlogEngine>();

            //Managers
            services.AddScoped<IBlogPostManager, BlogPostManager>();
        }
    }
}
