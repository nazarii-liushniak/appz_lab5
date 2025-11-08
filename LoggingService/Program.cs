using MassTransit;
using OpenTelemetry.Trace;
using LoggingService.Consumers;
using OpenTelemetry.Resources;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<ScheduleUpdatedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var host = hostContext.Configuration["RabbitMQ__Host"] ?? "rabbitmq";

                cfg.Host(host, "/", h =>
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
            });
        });

        services.AddOpenTelemetry().WithTracing(tracerProviderBuilder =>
        {
            tracerProviderBuilder.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("loggingservice"))
                                 .AddMassTransitInstrumentation()
                                 .AddOtlpExporter(o => o.Endpoint = new Uri("http://jaeger:4317"));
        });
    })
    .UseConsoleLifetime()
    .Build();

await builder.RunAsync();
