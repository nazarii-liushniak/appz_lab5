using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using TrainService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<TrainDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TrainDb")));

builder.Services.AddControllers();

builder.Services.AddOpenTelemetry().WithTracing(tracerProviderBuilder =>
{
    tracerProviderBuilder.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("trainservice"))
                         .AddAspNetCoreInstrumentation()
                         .AddEntityFrameworkCoreInstrumentation()
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