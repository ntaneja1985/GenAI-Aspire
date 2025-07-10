
using ServiceDefaults.Messaging;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.AddServiceDefaults();
builder.AddRedisDistributedCache(connectionName: "cache");
builder.Services.AddScoped<BasketService>();


//Register the HttpClient
builder.Services.AddHttpClient<CatalogApiClient>(client =>
{
    client.BaseAddress = new Uri("https+http://catalog");
});

//Scan the current assembly for consumers, sagas, and state machines and register them with MassTransit
builder.Services.AddMassTransitWithAssemblies(Assembly.GetExecutingAssembly());

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapDefaultEndpoints();
app.MapBasketEndpoints();

app.UseHttpsRedirection();

app.Run();


