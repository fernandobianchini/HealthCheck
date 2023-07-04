using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Weather.HealthCheck
{
    public static class ServiceRegistration
    {
        private const string UriHealthCheck = "/health";

        public static void AddHealthCheckService(this IServiceCollection services, IConfiguration _config)
        {
            services
                .AddHealthChecks()
                    .AddSqlServer(_config.GetConnectionString("defaultConnection"),
                                    name: "Banco de Dados Principal",
                                    tags: new[] { "SQL Server", "Local" })
                    .AddUrlGroup(new Uri("http://api1"), "API 1", tags: new[] { "API" });

            services
                .AddHealthChecksUI(options =>
                {
                    options.AddHealthCheckEndpoint("Healthcheck API", UriHealthCheck);
                })
                .AddInMemoryStorage();
        }

        public static void AddHealthCheckApp(this WebApplication app)
        {
            app.MapHealthChecks(UriHealthCheck, new()
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.MapHealthChecksUI(options =>
            {
                options.UIPath = "/dashboard";
                options.PageTitle = "HealthCheck Dashboard";
                options.AddCustomStylesheet("../Weather.HealthCheck/style.css");

            });
        }
    }
}