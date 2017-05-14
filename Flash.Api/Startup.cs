using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Flash.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddCors();
            services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase());
            //services.AddCors(option =>
            //{
            //    option.AddPolicy("Cors",
            //      builder => builder.AllowAnyOrigin()
            //         .AllowAnyMethod()
            //         .AllowAnyHeader());

            //});
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            app.UseCors(b =>
            b.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("this is the secret phrase"));

            app.UseJwtBearerAuthentication(
                new JwtBearerOptions
                {
                    AutomaticAuthenticate = true,
                    AutomaticChallenge = true,
                    TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        IssuerSigningKey = signingKey,
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = false,
                        ValidateAudience = false

                    }

                }
                );
            app.UseCors("Cors");
            app.UseMvc();
            SeedData(app.ApplicationServices.GetService<ApiContext>());

        }
        public void SeedData(ApiContext context)
        {
            context.messages.Add(
                new Models.Message
                {
                    Owner = "Amrinder",
                    Text = "Hi"
                });
            context.messages.Add(new Models.Message
            {
                Owner = "Ricky",
                Text = "Hello"
            });
            context.users.Add(new Models.User { Email = "a", FirstName = "Amrinder", Password = "a",Id="1" });
            context.SaveChanges();
        }
    }
}
