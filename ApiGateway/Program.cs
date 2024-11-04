using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

var ocelotConfigFileName = builder.Environment.IsDevelopment() ?
  "ocelot.Development.json" : "ocelot.Production.json";

builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
  .AddJsonFile(ocelotConfigFileName, optional: false, reloadOnChange: true)
  .AddEnvironmentVariables();

builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

await app.UseOcelot();

app.Run();