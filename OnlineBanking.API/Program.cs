using OnlineBanking.API.Extensions;
using OnlineBanking.Application;
using OnlineBanking.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
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
builder.Services.ConfigureApplicationServices(configuration);
builder.Services.ConfigureInfrastructureServices(configuration);
builder.Services.ConfigurePersistenceServices(configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    app.UseSwaggerDocumentation();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors("CorsPolicy");

app.MapControllers();

app.Run();
