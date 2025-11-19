using MassTransit;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using TrainService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<TrainDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TrainDb")));

builder.Services.AddHttpClient();
builder.Services.AddControllers();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumers(typeof(Program).Assembly);

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ__Host"] ?? "rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.UseDelayedRedelivery(r => r.Intervals(
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(20)
        ));

        cfg.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(10)));
        cfg.ConfigureEndpoints(context);
        cfg.UseInstrumentation();
    });
});

builder.Services.AddOpenTelemetry().WithTracing(tracerProviderBuilder =>
{
    tracerProviderBuilder.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("trainservice"))
                         .AddAspNetCoreInstrumentation()
                         .AddHttpClientInstrumentation()
                         .AddEntityFrameworkCoreInstrumentation()
                         .AddMassTransitInstrumentation()
                         .AddOtlpExporter(o => o.Endpoint = new Uri("http://jaeger:4317"));
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TrainDbContext>();
    db.Database.Migrate();
}

app.MapControllers();

app.Run();