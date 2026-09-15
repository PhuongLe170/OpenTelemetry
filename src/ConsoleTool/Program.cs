using System.Reflection;
using ConsoleTool;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using var traceProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(
        ResourceBuilder.CreateDefault()
            .AddService(
                serviceName: "ConsoleTool",
                serviceNamespace: "Dometrain.Courses.OpenTelemetry",
                serviceVersion: Assembly.GetExecutingAssembly().GetName().Version!.ToString())
    )
    .AddSource(ApplicationDiagnostics.ActivitySourceName)
    .AddConsoleExporter()
    .Build();

await DoWork();

Console.WriteLine("Done!");

static async Task DoWork()
{
    using var activity = ApplicationDiagnostics.ActivitySource.StartActivity("DoWork");

    await StepOne();
    await StepTwo();
}

static async Task StepOne()
{
    using var activity = ApplicationDiagnostics.ActivitySource.StartActivity("StepOne");

    await Task.Delay(500);
}

static async Task StepTwo()
{
    using var activity = ApplicationDiagnostics.ActivitySource.StartActivity("StepTwo");

    await Task.Delay(1000);
}
