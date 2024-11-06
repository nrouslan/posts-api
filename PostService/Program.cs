using AuthSDK;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using PostService.Data;
using EventBusSDK;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
      opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = JwtAuthOptions.GetSymmetricSecurityKey(),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,
      };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

var rabbitMQConStr = builder.Configuration.GetConnectionString("RabbitMQ");

Console.WriteLine($"--> RabbitMQ Connection string: {rabbitMQConStr}");

builder.Services.AddHostedService<MessageBusConsumer>();

builder.Services.AddSingleton<IEventProcessor, EventProcessor>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

if (builder.Environment.IsProduction())
{
  Console.WriteLine("--> Using PostgreSQL Db");

  builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostsDb")));
}
else
{
  Console.WriteLine("--> Using InMem Db");

  builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("InMem"));
}

builder.Services.AddScoped<IPostRepo, PostRepo>();

builder.Services.AddScoped<IPrincipalHelper, PrincipalHelper>();

builder.Services.AddHttpClient<IUsersDataClient, UsersDataClient>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
