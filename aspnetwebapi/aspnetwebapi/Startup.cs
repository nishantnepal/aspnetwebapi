using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace aspnetwebapi
{
    public class Startup
    {
        private string _version;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _version = Configuration["VersionPathBase"] ?? "v1";
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = _version });
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            
            app.UsePathBase($"/{_version}");

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", $"Demo API {_version}");
                c.RoutePrefix = string.Empty; // serve the Swagger UI at the app's root (http://localhost:<port>/)
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet($"/ping-live", async context =>
                {
                    await context.Response.WriteAsync("Live");
                }).WithMetadata(new AllowAnonymousAttribute());
                endpoints.MapGet($"/ping-ready", async context =>
                {
                    await context.Response.WriteAsync("Ready");
                }).WithMetadata(new AllowAnonymousAttribute());
            });
        }
    }
}
