using Microsoft.EntityFrameworkCore;
using UserService.AsyncDataServices;
using UserService.Data;
using UserService.SyncDataServices.Grpc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

Console.WriteLine("--> Using InMem Db");

builder.Services.AddDbContext<AppDbContext>(options =>
  options.UseInMemoryDatabase("InMem")
);

builder.Services.AddScoped<IUserRepo, UserRepo>();

builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

builder.Services.AddGrpc();

var app = builder.Build();

app.MapControllers();

app.MapGrpcService<GrpcUserService>();

app.MapGet("/protos/users.proto", async ctx =>
{
  await ctx.Response.WriteAsync(File.ReadAllText("Protos/users.proto"));
});

PrepDb.PrepPopulation(app, builder.Environment.IsProduction());

app.Run();
