
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

builder.Services.AddAuthentication()
    .AddKeycloakJwtBearer(
    serviceName: "keycloak",
    realm: "eshop", // The realm for the basket service
    configureOptions: options =>
    {
        options.RequireHttpsMetadata = false; // Set to true in production
        //options.Authority = "https://keycloak:8080/realms/eshop"; // Keycloak authority URL
        options.Audience = "account"; // The audience for the basket service
    });

builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapDefaultEndpoints();
app.MapBasketEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.Run();


