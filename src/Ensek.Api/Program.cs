using Ensek.Repository.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Ensek.Services.Services;

var builder = WebApplication.CreateBuilder(args);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy.WithOrigins("http://localhost:5174")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("EnsekDatabase") ?? throw new InvalidOperationException("Connection string to 'Ensek' database" + " not found.");
builder.Services.AddDbContext<EnsekContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IMeterReadingValidationService, MeterReadingValidationService>();
builder.Services.AddScoped<IMeterReadingService, MeterReadingService>();


builder.Services.AddMvc().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    opt.JsonSerializerOptions.PropertyNamingPolicy = null;
});

var app = builder.Build();


app.UseCors("AllowReactApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();