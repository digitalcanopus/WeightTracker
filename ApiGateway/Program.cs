using ApiGateway.Helpers;
using ApiGateway.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var microserviceEndpoints = ServiceEndpointsHelper.GetServiceEndpoints(builder.Configuration);
builder.Services.AddSingleton(microserviceEndpoints);
builder.Services.AddSingleton<HttpService>();

builder.Services.AddHttpClient();

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
