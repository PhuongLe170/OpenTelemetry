using System.Reflection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace RiskEvaluator.Extensions;

public static class OpenTelemetryConfigurationExtensions
{
    private const string ServiceName = "RiskEvaluator";

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
                    .AddConsoleExporter()
                    .AddOtlpExporter());

        return builder;
    }
}
