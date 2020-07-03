using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using WindowsAndJWTAuth.SecurityTokenHandlers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;

namespace WindowsAndJWTAuth
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
            services.AddCors();

            SecurityKey key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("asdjk89asdjaskldj09asdjaskldj0"));

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = IISDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme + "CF", options =>
                {
                    options.SaveToken = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        RequireExpirationTime = true,
                        RequireSignedTokens = true,
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateLifetime = false,
                        ValidIssuer = "https://rochegroup.cloudflareaccess.com",
                        ValidAudience = "67fe27aad12e11d52dbcf64a4cdef8eb320de877b0e9fe37b601bd5cecd2a894"
                    };

                    options.SecurityTokenValidators.Clear();
                    options.SecurityTokenValidators.Add(new CfJwtSecurityTokenHandler("https://stsdev.roche.com"));

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            string authorization = context.Request.Headers["Cf-Access-Jwt-Assertion"];

                            // If no authorization header found, nothing to process further
                            if (string.IsNullOrEmpty(authorization))
                            {
                                context.NoResult();
                                return Task.CompletedTask;
                            }

                            if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                            {
                                context.Token = authorization.Substring("Bearer ".Length).Trim();
                            }

                            // If no token found, no further work possible
                            if (string.IsNullOrEmpty(context.Token))
                            {
                                context.NoResult();
                                return Task.CompletedTask;
                            }

                            return Task.CompletedTask;
                        },
                    };

                })
                .AddCookie("CookieCF", options =>
                {
                    options.Cookie.Name = "Cf-Authorization";

                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };
                });

            //disable automatic authentication for out-of-process hosting
            services.Configure<IISOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Roche.STS.Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "WindowsVsJwt v1"); });
            
            app.UseWhen(
                context => !context.Request.Path.ToString().Contains("/api/WindowsAuthentication/GetToken"),
                builder => builder.UseMiddleware<ReplaceHttp401StatusCodeMiddleware>());

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
