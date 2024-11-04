using AuthService.Data;
using EventBusSDK;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var rabbitMQConStr = builder.Configuration.GetConnectionString("RabbitMQ");

builder.Services.AddHostedService<MessageBusSubscriber>();

builder.Services.AddSingleton<IEventProcessor, EventProcessor>();

Console.WriteLine($"--> RabbitMQ Connection string: {rabbitMQConStr}");


builder.Services.AddSingleton(sp => new ConnectionFactory()
{
  Uri = new Uri(rabbitMQConStr),
  DispatchConsumersAsync = true
});

builder.Services.AddSingleton<MessageBusClientService>();

builder.Services.AddSingleton<MessageBusPublisher>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

if (builder.Environment.IsProduction())
{
  Console.WriteLine("--> Using PostgreSQL Db");

  builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("UserAccountsDb")));
}
else
{
  Console.WriteLine("--> Using InMem Db");

  builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("InMem"));
}

builder.Services.AddScoped<IUserAccountRepo, UserAccountRepo>();

var app = builder.Build();

app.MapControllers();

app.Run();
