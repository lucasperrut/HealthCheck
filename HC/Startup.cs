using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.HC;
using WebApplication1.HC.Models;

namespace WebApplication1
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

            services.AddHealthChecks()
                .AddCheck<ExampleHC>("Custom Example")
                .AddSqlServer(Configuration.GetConnectionString("DefaultConnection"));
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

            app.UseHealthChecks("/health", new HealthCheckOptions()
            {
                ResponseWriter = HCResponseWriter
            });
        }

        private Task HCResponseWriter(HttpContext context, HealthReport result)
        {
            context.Response.ContentType = "application/json";

            var hcModel = new HCModel
            {
                Status = result.Status.ToString(),
                Results = result.Entries.Select(x => new HCModel.HCResultModel
                {
                    Name = x.Key,
                    Description = x.Value.Description,
                    Status = x.Value.Status.ToString()
                })
            };

            return context.Response.WriteAsync(JsonConvert.SerializeObject(hcModel, Formatting.Indented));

            //var json = new JObject(
            //    new JProperty("status", result.Status.ToString()),
            //    new JProperty("results", new JObject(result.Entries.Select(pair =>
            //        new JProperty(pair.Key, new JObject(
            //            new JProperty("status", pair.Value.Status.ToString()),
            //            new JProperty("description", pair.Value.Description),
            //            new JProperty("data", new JObject(pair.Value.Data.Select(
            //                p => new JProperty(p.Key, p.Value))))))))));
            //return context.Response.WriteAsync(
            //    json.ToString(Formatting.Indented));
        }
    }
}