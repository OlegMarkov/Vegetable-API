using Microsoft.AspNetCore.Builder;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Vegetable.API.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vegetable.Core.Database;
using Vegetable.API.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;
using Telegram.Bot;
using Vegetable.API.Middlewares;
using Vegetable.Core.Services;
using AspNetCore.Yandex.ObjectStorage.Extensions;
using Vegetable.Core.Extensions;
using Vegetable.API.Filters;
using Vegetable.Core.Storage;

namespace Vegetable.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            BotConfig = Configuration.GetSection("BotConfiguration").Get<BotConfiguration>();
            Auth0Config = Configuration.GetSection("Auth0").Get<Auth0Configuration>();
        }

        public IConfiguration Configuration { get; }
        private BotConfiguration BotConfig { get; }
        private Auth0Configuration Auth0Config { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Before anything else: secrets live in the environment now, not in
            // appsettings.json, so "nobody set it on this host" is the mistake
            // to expect. Fail here with a message that names the key, rather
            // than as a NullReferenceException on the first login attempt.
            foreach (string warning in RequiredSecrets.Validate(Configuration))
            {
                Console.WriteLine($"[config] {warning}");
            }

            services.AddAutoMapper(typeof(Startup));
            services.AddCors();
            services.AddYandexObjectStorage(options =>
            {
                options.BucketName = Configuration["YandexStorage:BucketName"];
                options.AccessKey = Configuration["YandexStorage:AccessKey"];
                options.SecretKey = Configuration["YandexStorage:SecretKey"];
            });
            services.AddDbContext<PostgreDbContext>(options =>
                options.UseNpgsql(Configuration["ConnectionStrings:Postgre"], b => b.MigrationsAssembly("Vegetable.Core")));
            services.AddHttpClient("tgwebhook")
                .AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(BotConfig.BotToken, httpClient));
            services.AddHttpClient();
            services.AddTransient<IOwnerRepo, OwnerRepo>();
            services.AddSingleton<IEmailService, EmailService>();
            services.AddTransient<IPaymentService, PaymentService>();
            services.AddSingleton<ISmsService, SmsService>();
            services.UsePushApi(Configuration);
            services.AddSingleton<IUserRepo>(new UserRepo(Auth0Config));
            services.AddTransient<ILogRepo, LogRepo>();
            services.AddTransient<IOrderRepo, OrderRepo>();
            services.AddTransient<INotificationMessageRepo, NotificationMessageRepo>();
            services.AddTransient<INotificationsService, NotificationsService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<ISettingsRepo, SettingsRepo>();
            services.AddTransient<ICallPasswordService, CallPasswordService>();
            services.AddTransient<IInternalCaptchaService, GoogleCaptchaService>();
            services.AddMemoryCache();
            services.AddTransient<IBotCommandRepository, BotCommandRepository>();
            services.AddScoped<QueryTokenFilter>();
            services.AddMvc(option => option.EnableEndpointRouting = false)
                .AddJsonOptions( option => {
                    option.JsonSerializerOptions.Converters.Add(new JsonTimeSpanConverter());
                    option.JsonSerializerOptions.Converters.Add(new JsonStringEnumMemberConverter());
                })
                .AddApplicationPart(Assembly.Load(new AssemblyName("Vegetable.API")));
            if (BotConfig.SetWebhook)
            {
                services.AddHostedService<ConfigureWebhook>();
                services.AddScoped<HandleUpdateService>();
            }
            services.AddControllers().AddNewtonsoftJson();
            // ConfigureAuth(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            if (BotConfig.SetWebhook) app.UseRouting();
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            // custom jwt auth middleware
            app.UseMiddleware<JwtMiddleware>();
           
            app.UseAuthentication();
            if (BotConfig.SetWebhook) app.UseEndpoints(endpoints =>
            {
                // Configure custom endpoint per Telegram API recommendations:
                // https://core.telegram.org/bots/api#setwebhook
                // If you'd like to make sure that the Webhook request comes from Telegram, we recommend
                // using a secret path in the URL, e.g. https://www.example.com/<token>.
                // Since nobody else knows your bot's token, you can be pretty sure it's us.
                var token = BotConfig.BotToken;
                endpoints.MapControllerRoute(name: "tgwebhook",
                                             pattern: $"bot/{token}",
                                             new { controller = "Webhook", action = "Post" });
                endpoints.MapControllers();
            });
            app.UseMvc();
        }

        protected virtual void ConfigureAuth(IServiceCollection services)
        {
            var domain = $"https://{Auth0Config.Domain}/";

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = domain;
                options.Audience = Auth0Config.ApiIdentifier;

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        // Add the access_token as a claim, as we may actually need it
                        if (context.SecurityToken is JwtSecurityToken accessToken)
                        {
                            if (context.Principal.Identity is ClaimsIdentity identity)
                            {
                                identity.AddClaim(new Claim("access_token", accessToken.RawData));
                            }
                        }

                        return Task.CompletedTask;
                    }
                };

            });
        }
    }
}
