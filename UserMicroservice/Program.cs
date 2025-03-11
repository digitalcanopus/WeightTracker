using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Text.Json;
using UserMicroservice.Services.RabbitMQ;
using UserMicroservice.Services.Tokens;
using UserMicroservice.Services.Users;
using UserMicroservice.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// Get configuration
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDB"));

// Register Mongo client
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var dbHostname = Environment.GetEnvironmentVariable("DATABASE_USER_HOSTNAME");
    var dbPort = Environment.GetEnvironmentVariable("DATABASE_PORT");
    var dbUsername = Environment.GetEnvironmentVariable("DATABASE_USERNAME");
    var dbPassword = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");

    var connectionString =
        (!string.IsNullOrEmpty(dbPort) && !string.IsNullOrEmpty(dbUsername) && !string.IsNullOrEmpty(dbPassword) && !string.IsNullOrEmpty(dbHostname))
        ? $"mongodb://{dbUsername}:{dbPassword}@{dbHostname}:{dbPort}/"
        : settings.ConnectionString;

    return new MongoClient(connectionString);
});

// Register the DB
builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    var env = Environment.GetEnvironmentVariable("DATABASE_NAME");
    return client.GetDatabase(env ?? settings.DatabaseName);
});

// JWT settings
var jwtSettings = new JwtSettings
{
    Key = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")! ?? builder.Configuration["JWT:Key"]!,
    Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")! ?? builder.Configuration["JWT:Issuer"]!,
    Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")! ?? builder.Configuration["JWT:Audience"]!,
    ExpMin = int.TryParse(Environment.GetEnvironmentVariable("JWT_EXP_MIN"), out var expMin)
        ? expMin
        : int.Parse(builder.Configuration["JWT:ExpMin"]!)
};

builder.Services.AddSingleton(jwtSettings);

// RabbitMQ settings
var rabbitMqSettings = new RabbitMqSettings
{
    HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOSTNAME")! ?? builder.Configuration["RabbitMQ:HostName"]!,
    Port = int.TryParse(Environment.GetEnvironmentVariable("RABBITMQ_PORT"), out var port)
        ? port
        : int.Parse(builder.Configuration["RabbitMQ:Port"]!),
    Username = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_USER")! ?? builder.Configuration["RabbitMQ:Username"]!,
    Password = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_PASS")! ?? builder.Configuration["RabbitMQ:Password"]!
};

builder.Services.AddSingleton(rabbitMqSettings);

// RabbitMQ configs
var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Configs", "brokersettings.json");
var fileContent = File.ReadAllText(filePath);
var brokerSettings = JsonSerializer.Deserialize<List<BrokerSettings>>(fileContent)!;

builder.Services.AddSingleton(brokerSettings);

builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();
builder.Services.AddSingleton<IUserEventPublisher, UserEventPublisher>();

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

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
