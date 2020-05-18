using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Scraper.API.Helpers;
using Scraper.API.Middleware;
using Scraper.Data;
using Scraper.Repositories;
using Scraper.Repositories.IRepositories;
using Scraper.Services;
using Scraper.Services.BackgroundServices;
using Scraper.Services.IServices;
using System;

namespace Scraper.API
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddAutoMapper(typeof(MappingProfile));

            services.AddScoped<IShowRepository, ShowRepository>();
            services.AddScoped<IShowService, ShowService>();

            services.AddHttpClient<IScraperService, ScraperService>(c =>
            {
                c.BaseAddress = new Uri("http://api.tvmaze.com/");
                c.DefaultRequestHeaders.Add("User-Agent", "HttpClient-TVmaze-API");
            })
              .AddPolicyHandler(PolicyProvider.WaitAndRetry());

            services.AddOpenApiDocument(config =>
            {
                config.Title = "Scraper API";
            });

            services.AddHostedService<ScrapingBackgroundService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMiddleware<ErrorHandlingMiddleware>();

            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }

            // Register the Swagger generator and the Swagger UI middlewares
            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseMvc();
        }
    }
}