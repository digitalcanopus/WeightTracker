using Microsoft.Extensions.Options;
using MongoDB.Driver;
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
