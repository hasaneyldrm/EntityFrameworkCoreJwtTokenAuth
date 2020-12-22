using System;
using System.Text;
using EntityFrameworkCoreJwtTokenAuth.Context;
using EntityFrameworkCoreJwtTokenAuth.Interfaces;
using EntityFrameworkCoreJwtTokenAuth.Services;
using EntityFrameworkCoreJwtTokenAuth.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace EntityFrameworkCoreJwtTokenAuth
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {

            #region DbContext

            services.AddDbContext<DatabaseContext>(optionsAction: options =>
                options.UseSqlServer(connectionString: Configuration.GetConnectionString(name: "MyConnection")));

            #endregion

            #region Swagger

            services.AddSwaggerGen(setupAction: c =>
            {

                c.SwaggerDoc(name: "v1", info: new OpenApiInfo { Title = "EntityFrameworkCoreJwtTokenAuth", Version = "v1" });

                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //c.IncludeXmlComments(xmlPath);
            });

            #endregion

            services.AddCors(setupAction: o => o.AddPolicy(name: "MyPolicy", configurePolicy: builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

            var appSettingsSection = Configuration.GetSection(key: "AppSettings");
            services.Configure<AppSettings>(config: appSettingsSection);
            services.AddMemoryCache();
            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(s: appSettings.Secret);
            services.AddAuthentication(configureOptions: x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(configureOptions: x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key: key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddScoped<IUserService, UserService>();

            services.AddControllers();

        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();
                app.UseSwaggerUI(setupAction: c =>
                {
                    c.SwaggerEndpoint(url: "v1/swagger.json", name: "MyAPI V1");
                });
            }

            else
            {
                app.UseExceptionHandler(errorHandlingPath: "/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors(policyName: "MyPolicy");
            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI(setupAction: c =>
            {
                c.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "Swagger");

            });
            app.UseAuthorization();

            app.UseEndpoints(configure: endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

    }
}
