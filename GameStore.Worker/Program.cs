using MassTransit;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
{
    services.AddMassTransit(x =>
    {
        x.AddConsumer<PaymentProcessorConsumer>();

        x.UsingRabbitMq((context, cfg) =>
        {
            var rabbitHost = hostContext.Configuration["RabbitMQ:Host"] ?? "localhost";
            cfg.Host(rabbitHost, "/", h => {
                h.Username("guest");
                h.Password("guest");
            });
            
            cfg.ConfigureEndpoints(context); 
        });
    });
});

Directory.CreateDirectory("/app/shared");

var host = builder.Build();
host.Run();