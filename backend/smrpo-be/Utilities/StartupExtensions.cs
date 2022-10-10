using AutoMapper;
using smrpo_be.Data;
using smrpo_be.Services;
using smrpo_be.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace smrpo_be
{
    public static class StartupExtensions
    {
        public static void AddDbContextConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("SmrpoDatabase");
            services.AddDbContext<SmrpoContext>(options => options.UseMySql(connectionString));
        }

        public static void AddHealthChecksConfiguration(this IServiceCollection services)
        {
            services.AddHealthChecks().AddDbContextCheck<SmrpoContext>();
        }

        public static void AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(document =>
            {
                document.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "smrpo_be",
                    Description = "smrpo_be API",
                    Contact = new OpenApiContact
                    {
                        Name = "smrpo_be",
                        Email = "it@smrpo_be.io",
                        Url = new System.Uri("https://smrpo_be.io")
                    }
                });

                document.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. \n" +
                    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjA4ZDdjY2MxLWE2MDAtNDBlOS04OThlLTVlNTRhYmNjOTAzMCIsIm5iZiI6MTU4NDcyMTczMiwiZXhwIjoxNTg1MzI2NTMyLCJpYXQiOjE1ODQ3MjE3MzIsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjUwMDEiLCJhdWQiOiJodHRwczovL2xvY2FsaG9zdDo1MDAxIn0.1o5GYfTbpKGKtb481gd4Q6ITs4TD9JG037JGx-Voo7k \n" +
                    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjA4ZDdjY2MxLWE2MTQtNGNiNy04OTMwLTAyYWYxYmIyZWZiOCIsIm5iZiI6MTU4NDcyMTkwMSwiZXhwIjoxNTg1MzI2NzAxLCJpYXQiOjE1ODQ3MjE5MDEsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjUwMDEiLCJhdWQiOiJodHRwczovL2xvY2FsaG9zdDo1MDAxIn0.bWXXLP2F0nLAw9sJrkVzJCml2Vb_dH1kyQh6hLBPHmo",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "bearer",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT"
                });
                document.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new List<string>()
                    }
                });
            });
        }


        public static void AddSmrpoServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IUserStoryService, UserStoryService>();
            services.AddScoped<ISprintService, SprintService>();
            services.AddScoped<ITaskService, TaskService>();
        }

        public static void AddAuthorizationConfiguration(this IServiceCollection services)
        {
            services.AddAuthorization(x =>
            {
                x.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
                    .RequireAuthenticatedUser().Build());
            });
        }

        public static void SetupSettings(this IServiceCollection services, IConfiguration configuration)
        {

        }

        public static void AddAuthenticationConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var tokenSettings = configuration.GetSection("TokenValidation");
            services.Configure<TokenValidation>(tokenSettings);
            // configure jwt authentication
            var tokenOptions = tokenSettings.Get<TokenValidation>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = tokenOptions.ValidateIssuer,
                    ValidateAudience = tokenOptions.ValidateAudience,
                    ValidateLifetime = tokenOptions.ValidateLifetime,
                    ValidateIssuerSigningKey = tokenOptions.ValidateIssuerSigningKey,
                    ValidIssuer = tokenOptions.ValidIssuer,
                    ValidAudience = tokenOptions.ValidAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.IssuerSigningKey))
                };
            });
        }


        public static void AddAutomapperConfiguration(this IServiceCollection services)
        {
            IMapper mapper = Assembly.GetExecutingAssembly().CreateMapper();
            services.AddSingleton(mapper);
        }

        public static void AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var allowedCors = configuration.GetSection("AllowedCors").Get<string[]>();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowCors",
                    builder =>
                    {
                        builder
                            .WithOrigins(allowedCors)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
            });
        }

        public static IApplicationBuilder UseHealthCheckConfiguration(this IApplicationBuilder app)
        {
            app.UseHealthChecks("/appStatus", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) => {
                    var status = report.Status.ToString();
                    switch (report.Status)
                    {
                        case Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy:
                            status = "APP_STATUS_OK";
                            break;
                        case Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded:
                            status = "APP_STATUS_DEGRADED";
                            break;
                        case Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy:
                        default:
                            status = "APP_STATUS_FAILURE";
                            break;
                    }
                    var bytes = Encoding.UTF8.GetBytes(status);
                    context.Response.ContentType = "text/plain";
                    await context.Response.Body.WriteAsync(bytes);
                }
            });

            return app;
        }
    }
}
