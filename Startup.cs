using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace ServerPS
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Set up the OAUTH service and configured it
            services.AddAuthentication("OAuth")
               .AddJwtBearer("OAuth", config =>
               {
                   var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);
                   var key = new SymmetricSecurityKey(secretBytes);

                   config.Events = new JwtBearerEvents()
                   {
                       OnMessageReceived = context =>
                       {
                           if (context.Request.Query.ContainsKey("access_token"))
                           {
                               context.Token = context.Request.Query["access_token"];
                           }

                           return Task.CompletedTask;
                       }
                   };
                   // this is how we receive the access token and then going to validate it in
                   // a certain way then that allow us to authorization end points 
                   config.TokenValidationParameters = new TokenValidationParameters()
                   {
                       ClockSkew = TimeSpan.Zero,
                       ValidIssuer = Constants.Issuer,
                       ValidAudience = Constants.Audiance,
                       IssuerSigningKey = key,
                   };
               });

            services.AddControllersWithViews().AddRazorRuntimeCompilation();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
