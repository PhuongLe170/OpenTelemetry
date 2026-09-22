using System.Reflection;
using OpenTelemetry.Resources;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Notifications.Extensions;

public static class OpenTelemetryConfigurationExtensions
{
    private const string ServiceName = "Notifications";

    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource
                    .AddService(
                        serviceName: ServiceName,
                        serviceNamespace: "Dometrain.Courses.OpenTelemetry")
                    .AddAttributes(new[]
                    {
                        new KeyValuePair<string, object>("service.version",
                            Assembly.GetExecutingAssembly().GetName().Version!.ToString())
                    });
            })
            .WithTracing(tracing =>
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddConsoleExporter()
                    .AddOtlpExporter())
            .WithMetrics(metrics =>
                metrics
                    .AddAspNetCoreInstrumentation()
                    // Built-in meters shipped by ASP.NET Core / Kestrel
                    .AddMeter("Microsoft.AspNetCore.Hosting")
                    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                    .AddOtlpExporter());

        return builder;
    }
}
