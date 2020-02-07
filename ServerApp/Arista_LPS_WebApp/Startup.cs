using Arista_LPS_WebApp.DataProvider;
using Arista_LPS_WebApp.Extensions;
using Arista_LPS_WebApp.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NLog;
using System;
using System.IO;
using System.Text;

namespace Arista_LPS_WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();

            services.ConfigureLoggerService();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            if (string.Compare(appSettings.SSO, "FALSE", true) == 0)
            {
                services.AddCors();

                // configure jwt authentication
                var key = Encoding.ASCII.GetBytes(appSettings.Secret);
                services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            }
            else  // SSO
            {
                services.Configure<IISOptions>(options =>
                {
                    options.AutomaticAuthentication = true;
                });

                services.AddAuthentication(IISDefaults.AuthenticationScheme);
                services.AddCors(options =>
                {
                    options.AddPolicy("MyAllowSpecificOrigins",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:4200");
                        // builder.WithOrigins("http://10.30.10.102:4200");
                        builder.WithOrigins("http://192.168.70.157:4200");
                        // builder.WithOrigins("http://216.45.140.171:4200");
                        // builder.WithOrigins("http://192.168.74.171:4200");
                        // builder.WithOrigins("*");
                        builder.SetIsOriginAllowedToAllowWildcardSubdomains();
                        builder.WithHeaders("*");
                        builder.WithMethods("*");
                        builder.AllowCredentials();
                    });
                });

                services.AddHttpContextAccessor();
            }

            // configure DI for application services
            services.AddScoped<IUserDataProvider, UserDataProvider>();
            services.AddScoped<IDepartmentsDataProvider, DepartmentsDataProvider>();
            services.AddScoped<IRoleDataProvider, RoleDataProvider>();
            services.AddScoped<ICustomerDataProvider, CustomerDataProvider>();
            services.AddScoped<IApplicationDataProvider, ApplicationDataProvider>();
            services.AddScoped<IHomeDataProvider, HomeDataProvider>();
            services.AddScoped<IPostageDataProvider, PostageDataProvider>();
            services.AddScoped<IFeeDescriptionDataProvider, FeeDescriptionDataProvider>();
            services.AddScoped<IAdditionalChargesDataProvider, AdditionalChargesDataProvider>();
            services.AddScoped<IStateDataProvider, StateDataProvider>();
            services.AddScoped<IShipmentMethodDataProvider, ShipmentMethodDataProvider>();
            services.AddScoped<IRunningSummaryDataProvider, RunningSummaryDataProvider>();
            services.AddScoped<IInsertDataProvider, InsertDataProvider>();
            services.AddScoped<IContactsDataProvider, ContactsDataProvider>();
            services.AddScoped<IServiceAgreementDataProvider, ServiceAgreementDataProvider>();
            services.AddScoped<IRateDescriptionDataProvider, RateDescriptionDataProvider>();
            services.AddScoped<IChangePrintLocationDataProvider, ChangePrintLocationDataProvider>();

            services.AddScoped<IApplicationDataProvider, ApplicationDataProvider>();
            services.AddScoped<IAppTypeDataProvider, AppTypeDataProvider>();
            services.AddScoped<IApplicationNotificationsDataProvider, ApplicationNotificationsDataProvider>();
            services.AddScoped<IFlagDataProvider, FlagDataProvider>();
            services.AddScoped<IPerfPatternDataProvider, PerfPatternDataProvider>();
            services.AddScoped<ISizeDataProvider, SizeDataProvider>();
            services.AddScoped<ISoftwareDataProvider, SoftwareDataProvider>();
			services.AddScoped<IReportsDataProvider, ReportsDataProvider>();
            services.AddScoped<IPRateDataProvider, PRateDataProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var appSettingsSection = Configuration.GetSection("AppSettings");
            var appSettings = appSettingsSection.Get<AppSettings>();

            if (string.Compare(appSettings.SSO, "FALSE", true) == 0)
            {
                app.UseCors(x => x
               .AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
            }
            else
            {
                app.UseCors("MyAllowSpecificOrigins");
                app.UseHttpsRedirection();
            }

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}