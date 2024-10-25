using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using UserService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

Console.WriteLine("--> Using InMem Db");

builder.Services.AddDbContext<AppDbContext>(options =>
  options.UseInMemoryDatabase("InMem")
);

builder.Services.AddScoped<IUserRepo, UserRepo>();

var app = builder.Build();

app.MapControllers();

PrepDb.PrepPopulation(app, builder.Environment.IsProduction());

app.Run();
