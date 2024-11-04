using AuthService.Data;
using AuthService.EventProcessing;
using EventBusSDK;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

builder.Services.AddControllers();

builder.Services.AddHostedService<MessageBusSubscriber>();

builder.Services.AddSingleton<IEventProcessor, EventProcessor>();

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
