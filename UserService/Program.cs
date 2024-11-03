using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using AuthSDK;
using UserService.EventProcessing;
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

builder.Services.AddHostedService<MessageBusSubscriber>();

builder.Services.AddSingleton<IEventProcessor, EventProcessor>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

Console.WriteLine("--> Using InMem Db");

builder.Services.AddDbContext<AppDbContext>(options =>
  options.UseInMemoryDatabase("InMem")
);

// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseNpgsql(builder.Configuration.GetConnectionString("usersdb")));

builder.Services.AddScoped<IUserRepo, UserRepo>();

builder.Services.AddScoped<IPrincipalHelper, PrincipalHelper>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
