using CommentsSPATask.Api.Endpoints;
using CommentsSPATask.Api.Extensions;
using CommentsSPATask.Application;
using CommentsSPATask.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddApi(builder.Configuration);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("Swagger:Enabled"))
{
    app.UseSwaggerDocumentation();
    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
}

app.ApplyMigrations();

app.UseHttpsRedirection();

app.UseCors("Client");

app.UseUploadedFiles();

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseCustomExceptionHandler();

app.MapEndpoints();

app.Run();
