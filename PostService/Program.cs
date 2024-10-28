using Microsoft.EntityFrameworkCore;
using PostService;
using PostService.AsyncDataServices;
using PostService.Data;
using PostService.EventProcessing;
using PostService.SyncDataServices.Grpc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHostedService<MessageBusSubscriber>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IUserDataClient, UserDataClient>();

Console.WriteLine("--> Using InMem Db");

builder.Services.AddDbContext<AppDbContext>(options =>
  options.UseInMemoryDatabase("InMem")
);

builder.Services.AddScoped<IPostRepo, PostRepo>();

builder.Services.AddSingleton<IEventProcessor, EventProcessor>();

var app = builder.Build();

app.MapControllers();

PrepDb.PrepPopulation(app, builder.Environment.IsProduction());

app.Run();
