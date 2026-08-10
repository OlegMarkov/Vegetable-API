using AspNetCore.Yandex.ObjectStorage.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Vegetable.API;

namespace Vegetable.UnitTests
{
    public class TestStartup : Startup
    {

        public static IConfiguration TestConfiguration { get; set; }

        public static Guid CurrentOwner { get; set; }

        public TestStartup(IConfiguration configuration) : base(configuration)
        {
            TestConfiguration = configuration;
        }

        public override void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            // custom jwt auth middleware
            app.UseMiddleware<TestJwtMiddleware>();

            app.UseAuthentication();
            app.UseMvc();
        }

        protected override void ConfigureAuth(IServiceCollection services)
        {
            services.AddYandexObjectStorage(options =>
            {
                options.BucketName = "obs";
                options.AccessKey = "1YsfSIGs8TLPOyiYYFbE";
                options.SecretKey = "yUpN9rDmMjmL2Z9vy2lbpwB7HCke0miTxgWhtu6s";
            });
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test Scheme";
                options.DefaultChallengeScheme = "Test Scheme";
            }).AddTestAuth(o => { });
        }
    }
}

