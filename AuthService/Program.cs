using System.Reflection;
using AuthService.Data;
using EventBusSDK;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var rabbitMQConStr = builder.Configuration.GetConnectionString("RabbitMQ");

Console.WriteLine($"--> RabbitMQ Connection string: {rabbitMQConStr}");

builder.Services.AddHostedService<MessageBusConsumer>();

builder.Services.AddSingleton<IEventProcessor, EventProcessor>();

builder.Services.AddTransient<MessageBusPublisher>();

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

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
  var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
  options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

  options.SwaggerDoc("v1", new OpenApiInfo
  {
    Version = "v1",
    Title = "Auth Service",
    Description = "API сервиса для аутентификация пользователя системы."
  });

  options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
    Description = "Введите JWT токен авторизации.",
    Name = "Authorization",
    In = ParameterLocation.Header,
    Type = SecuritySchemeType.ApiKey,
    BearerFormat = "JWT",
    Scheme = "Bearer"
  });

  options.AddSecurityRequirement(new OpenApiSecurityRequirement()
  {
      {
        new OpenApiSecurityScheme
        {
          Reference = new OpenApiReference
          {
              Type = ReferenceType.SecurityScheme,
              Id = "Bearer"
          },
        },
        new List<string>()
      }
  });
});

var app = builder.Build();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.Run();
