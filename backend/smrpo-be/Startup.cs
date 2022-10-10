using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace smrpo_be
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.SetupSettings(Configuration);
            services.AddCorsConfiguration(Configuration);
            services.AddControllers();
            services.AddDbContextConfiguration(Configuration);
            services.AddHealthChecksConfiguration();
            services.AddSwaggerConfiguration();
            services.AddAuthenticationConfiguration(Configuration);
            services.AddAuthorizationConfiguration();
            services.AddAutomapperConfiguration();
            services.AddSmrpoServices();
            services.AddControllers()
                .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } else
            {
                app.UseHttpsRedirection();
                app.UseHsts();
            }


            app.UseCors("AllowCors")
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseHealthCheckConfiguration()
                .UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Smrpo API v1");
                })
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
        }
    }
}
