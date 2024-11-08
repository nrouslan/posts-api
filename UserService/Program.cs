using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using AuthSDK;
using EventBusSDK;
using System.Reflection;
using Microsoft.OpenApi.Models;

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

builder.Services.AddTransient<MessageBusPublisher>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

if (builder.Environment.IsProduction())
{
  Console.WriteLine("--> Using PostgreSQL Db");

  var postgreSQLConStr = builder.Configuration.GetConnectionString("UsersDb");

  Console.WriteLine($"--> PostgreSQL Connection string: {postgreSQLConStr}");

  builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(postgreSQLConStr));
}
else
{
  Console.WriteLine("--> Using InMem Db");

  builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("InMem"));
}

builder.Services.AddScoped<IUserRepo, UserRepo>();

builder.Services.AddScoped<IPrincipalHelper, PrincipalHelper>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
  var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
  options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

  options.SwaggerDoc("v1", new OpenApiInfo
  {
    Version = "v1",
    Title = "User Service",
    Description = "API сервиса по работе с пользователями системы."
  });

  options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
    In = ParameterLocation.Header,
    Name = "Authorization",
    Type = SecuritySchemeType.Http,
    BearerFormat = "JWT",
    Scheme = "Bearer",
    Description = "Введите JWT токен авторизации.",
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.Run();
