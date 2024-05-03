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
            #region LOAD GLOBAL VAR & SETTINGS
            
            #endregion

            #region CHECK IF WALLET FILE IS NOT PRESENT CREATE NEW 
            if (!File.Exists(Path.Combine(System.AppContext.BaseDirectory, "myWallet.json")))
            {
                logger.Info("No myWallet.json file, creating new");
                try
                {
                    Library.Wallet walletH = new Library.Wallet();
                    Key_Pair wallet = walletH.GenPubPk();

                    dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(@"{'my_pkx':'" + wallet.PrivateKey_Hex 
                        + "','my_pubx':'" + wallet.PublicKey_Hex + "','my_addr':'" + wallet.Address_String + "','my_mnem':'" + wallet.Mnemonic_12_Words + "'}");
                    string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText(Path.Combine(System.AppContext.BaseDirectory, "myWallet.json"), output);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
            #endregion

            services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ConnectionString")));
            services.AddScoped<INodeRepo, NodeRepo>();

            services.AddScoped<ILibraryWallet, Library.Wallet>();
            services.AddScoped<IMySettings, MySettings>();

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
