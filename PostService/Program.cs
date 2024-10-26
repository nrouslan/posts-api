using Microsoft.EntityFrameworkCore;
using PostService;
using PostService.AsyncDataServices;
using PostService.Data;
using PostService.EventProcessing;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHostedService<MessageBusSubscriber>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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
