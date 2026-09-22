using System.Reflection;
using Npgsql;
using OpenTelemetry.Resources;
using Clients.Api.Diagnostics;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Clients.Api.Extensions;

public static class OpenTelemetryConfigurationExtensions
{
    private const string ServiceName = "Clients.Api";

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
                    .AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddNpgsql()
                    .AddRedisInstrumentation()
                    .AddConsoleExporter()
                    .AddOtlpExporter())
            .WithMetrics(metrics =>
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    // Built-in meters shipped by ASP.NET Core / Kestrel
                    .AddMeter("Microsoft.AspNetCore.Hosting")
                    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                    .AddMeter(ApplicationDiagnostics.Meter.Name)
                    .AddOtlpExporter());

        return builder;
    }
}
