using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

// OpenTelemetry
builder.Services.AddOpenTelemetry().WithTracing(tracerProviderBuilder =>
{
    tracerProviderBuilder.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("apigateway"))
                         .AddAspNetCoreInstrumentation()
                         .AddOtlpExporter(o => o.Endpoint = new Uri("http://jaeger:4317"));
});

var app = builder.Build();

app.UseRouting();

await app.UseOcelot();

app.Run();
