using System.Reflection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Reporting.Extensions;

public static class OpenTelemetryConfigurationExtensions
{
    private const string ServiceName = "Reporting";

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
                    .AddOtlpExporter());

        return builder;
    }
}
