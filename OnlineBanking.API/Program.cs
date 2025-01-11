using Microsoft.AspNetCore.Mvc;
using OnlineBanking.API.Extensions;
using OnlineBanking.API.Middleware;
using OnlineBanking.Application;
using OnlineBanking.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
});
builder.Services.AddSwaggerDocumentation();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
    builder => builder.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
    );
});

// Add services to the container.
builder.Services.ConfigureIdentityServices(configuration);
builder.Services.ConfigureApplicationServices();
builder.Services.ConfigureInfrastructureServices();
builder.Services.ConfigurePersistenceServices(configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    app.UseSwaggerDocumentation();

app.UseMiddleware<ExceptionHandlingMiddleware>(); 

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors("CorsPolicy");

app.MapControllers();

app.Run();
