using EKoin.Services;
using EKoin.Utility;
using Library;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Models;
using Models.DB;
using NLog;
using System;
using System.IO;
using System.Net.Http;
using static Models.Wallet;

namespace EKoin
{
    public class Startup
    {
        #region CTOR
        private readonly IConfiguration Configuration;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public IWebHostEnvironment Environment { get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }
        #endregion

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ConnectionString")));
            services.AddScoped<INodeRepo, NodeRepo>();
            services.AddScoped<ILedgerRepo, LedgerRepo>();
            services.AddScoped<IBalanceRepo, BalanceRepo>();

            services.AddSingleton<ILibraryWallet, Library.Wallet>();
            services.AddSingleton<IMySettings, MySettings>();
            //services.AddSingleton<IHttpClientFactory>();
            services.AddHttpClient();
            services.AddSingleton<IHttpRequest, HttpRequest>();
            services.AddHostedService<LoadingNodeService>();
            
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EKoin", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EKoin v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
