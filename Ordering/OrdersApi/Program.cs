﻿using MassTransit;
using Messaging.InterfacesConstants.Constants;
using Microsoft.EntityFrameworkCore;
using OrdersApi.Messages.Consumers;
using OrdersApi.Persistence;
using OrdersApi.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();

// Add services to the container.
//builder.Services.AddMassTransit(
//    c =>
//    {
//        c.AddConsumer<RegisterOrderCommandConsumer>();
//        //c.UsingRabbitMq((context, cfg) =>
//        //{
//        //    cfg.ConfigureEndpoints(context);
//        //});
//    }
//    );
//builder.Services.AddSingleton(
//    provider => Bus.Factory.CreateUsingRabbitMq(
//        cfg =>
//        {
//            cfg.Host("localhost", "/", h => { });
//            cfg.ReceiveEndpoint(RabbitMqMassTransitConstants.RegisterOrderCommandQueue, e =>
//            {
//                e.PrefetchCount = 16;
//                e.UseMessageRetry(x => x.Interval(2, TimeSpan.FromSeconds(10)));
//                e.Consumer<RegisterOrderCommandConsumer>(provider);
//            });
//            //cfg.ConfigureEndpoints(provider);
//        }
//        )
//    );
builder.Services.AddDbContext<OrderContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("OrdersContextConnection")));
builder.Services.AddTransient<IOrderRepository, OrderRepository>();



//var rabbitMqServiceBus = Bus.Factory.CreateUsingRabbitMq(
//    cfg =>
//    {
//        cfg.Host("localhost", "/", h => { });
//        cfg.ReceiveEndpoint(RabbitMqMassTransitConstants.RegisterOrderCommandQueue, e => 
//        {
//            e.PrefetchCount = 16;
//            e.UseMessageRetry(x => x.Interval(2, TimeSpan.FromSeconds(10)));
//            e.Consumer<RegisterOrderCommandConsumer>();
//        });
//    });
//builder.Services.AddMassTransit(
//    config =>
//    {
//        config.AddConsumer<RegisterOrderCommandConsumer>();
//        config.AddBus(provider => rabbitMqServiceBus);
//        //config.UsingRabbitMq((context, cfg) => cfg.ConfigureEndpoints(context));

//    });


builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<RegisterOrderCommandConsumer>();
    x.AddConsumer<OrderDispatchedEventConsumer>();
    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        cfg.Host("localhost", "/", h => { });
        cfg.ReceiveEndpoint(RabbitMqMassTransitConstants.RegisterOrderCommandQueue, ep =>
        {
            ep.PrefetchCount = 16;
            ep.UseMessageRetry(r => r.Interval(2, TimeSpan.FromSeconds(10)));
            ep.ConfigureConsumer<RegisterOrderCommandConsumer>(provider);
        });
        cfg.ReceiveEndpoint(RabbitMqMassTransitConstants.OrderDispatchedServiceQueue, ep =>
        {
            ep.PrefetchCount = 16;
            ep.UseMessageRetry(r => r.Interval(2, TimeSpan.FromSeconds(10)));
            ep.ConfigureConsumer<OrderDispatchedEventConsumer>(provider);
        });
    }));
});

//builder.Services.AddSingleton<IHostedService, BusService>();
//builder.Services.AddSingleton<IBus>(rabbitMqServiceBus);

builder.Services.AddSingleton<IHostedService, BusService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .SetIsOriginAllowed((host) => true)
        .AllowCredentials());
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("CorsPolicy");
app.UseAuthorization();

app.MapControllers();

app.Run();
