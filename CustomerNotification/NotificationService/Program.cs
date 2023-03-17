using MassTransit;
using Messaging.InterfacesConstants.Constants;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationService.Consumers;
using NotificationService.Services;

class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var host = CreateHostBuilder(args).Build();
        await host.RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var hostBuilder = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) => 
            {
                services.AddMassTransit(x =>
                {
                    x.AddConsumer<OrderProcessedEventConsumer>();
                    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                    {
                        cfg.Host("localhost", "/", h => { });
                        cfg.ReceiveEndpoint(RabbitMqMassTransitConstants.NotificationServiceQueue, ep =>
                        {
                            ep.PrefetchCount = 16;
                            ep.UseMessageRetry(r => r.Interval(2, TimeSpan.FromSeconds(10)));
                            ep.ConfigureConsumer<OrderProcessedEventConsumer>(provider);
                        });
                    }));
                });
                services.AddSingleton<IHostedService, BusService>();
            });
        return hostBuilder;
    }
}