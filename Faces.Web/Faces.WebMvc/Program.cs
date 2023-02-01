using Faces.WebMvc.Services;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

var rabbitMqServiceBus = Bus.Factory.CreateUsingRabbitMq(
    cfg =>
    {
        cfg.Host("localhost", "/", h => { });
    });
builder.Services.AddMassTransit(
    config =>
    {
        config.AddBus(provider => rabbitMqServiceBus);
    });
builder.Services.AddSingleton<IHostedService, BusService>();
builder.Services.AddSingleton<IBus>(rabbitMqServiceBus);



builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
