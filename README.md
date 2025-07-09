# GenAI-Aspire
.NET Aspire and Generative AI
- This is what we will build:
- ![alt text](image.png)
- .Net Aspire simplifies building distributed enterprise ready applications by providing a clean project structure and built in solution for configuration logging and so on.
- ![alt text](image-1.png)
- We will deploy to Azure Container Apps
- We will use Microsoft.Extensions.AI package and Semantic Kernel Library to add GenAI capabilities.
- Here Catalog and Basket communicate with each other using .Net Aspire
- ![alt text](image-2.png)
- We will build a Blazor Application for the UI
- This will also be deployed to Azure Container Apps
- We will use Semantic Search using Vector Database.
- This is the project structure:
- ![alt text](image-3.png)

## What is Cloud Native Distributed Architecture
- ![alt text](image-4.png)
- ![alt text](image-5.png)
- ![alt text](image-6.png)
- ![alt text](image-7.png)
- ![alt text](image-8.png)
- ![alt text](image-9.png)

## What is .NET Aspire
- ![alt text](image-10.png)
- ![alt text](image-11.png)
- ![alt text](image-12.png)

### Core Concepts of .NET Aspire
- ![alt text](image-13.png)
- ![alt text](image-14.png)
- ![alt text](image-15.png)
- ![alt text](image-16.png)
- In .Net Aspire, we can say that orchestration ensures each service and backing services is containerized and started in the correct order
- .Net Aspire Integrations lets you effortlessly add databases, caches, messaging systems or identity providers as a backing services
- Service discovery binds everything together, allowing microservices to find each other without manual configuration.
- This synergy makes .Net Aspire an excellent choice for the teams who want to build cloud native and event driven distributed solutions quickly and reliably.

### .Net Aspire Integration
- ![alt text](image-17.png)
- ![alt text](image-18.png)
- ![alt text](image-19.png)
- ![alt text](image-20.png)
- ![alt text](image-21.png)
- ![alt text](image-22.png)

### .Net Aspire Integrations - Built in Connectors
- ![alt text](image-23.png)
- ![alt text](image-24.png)
- ![alt text](image-25.png)
- ![alt text](image-26.png)
- Hosting Integration is all about provisioning and attaching resources
- Client integration is all about reading environment variables and connection strings and setting up connection with the provisioned resources. It automatically sets us telemetry, logging and health checks
- ![alt text](image-28.png)
- ![alt text](image-29.png)
- ![alt text](image-30.png)
- ![alt text](image-31.png)
- ![alt text](image-32.png)
- ![alt text](image-33.png)
- ![alt text](image-34.png)

### .Net Aspire Service Discovery
- ![alt text](image-35.png)
- ![alt text](image-36.png)
- ![alt text](image-37.png)
- ![alt text](image-38.png)
- ![alt text](image-39.png)

## Building the first .NET Aspire Application
- We will create a project using .NET Aspire Starter Template
- This is what is added by default in Program.cs:
```c#
var builder = DistributedApplication.CreateBuilder(args);

//Add Redis container resource
var cache = builder.AddRedis("cache");

//Register the minimal Api service and label it
var apiService = builder.AddProject<Projects.AspireSample_ApiService>("apiservice");


//Register the frontend app and label it and tie up its dependencies
builder.AddProject<Projects.AspireSample_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();



```

- ![alt text](image-40.png)
- ![alt text](image-41.png)
- ![alt text](image-42.png)
- AppHost project is the central hub for all projects and servers
- It should be the startup project
- ![alt text](image-43.png)
- Aspire.SampleDefaults is a shared project and contains the cross cutting concerns
- ![alt text](image-44.png)
- ApiService is an AspNet.Core minimal service project
- ![alt text](image-45.png)
- Aspire.Web is a blazor project and houses UI components. It depends on service defaults for cross cutting concerns
- Communicates with apiService
- ![alt text](image-46.png)
- ![alt text](image-48.png)
- AppHost project is the orchestrator project
- csproj file of AppHost project is as follows:
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <Sdk Name="Aspire.AppHost.Sdk" Version="9.0.0" />

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsAspireHost>true</IsAspireHost>
    <UserSecretsId>e9d520a0-d2c5-457e-9a71-c80f11410e88</UserSecretsId>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\AspireSample.ApiService\AspireSample.ApiService.csproj" />
    <ProjectReference Include="..\AspireSample.Web\AspireSample.Web.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Aspire.Hosting.AppHost" Version="9.0.0" />
    <PackageReference Include="Aspire.Hosting.Redis" Version="9.0.0" />
  </ItemGroup>

</Project>

```
- We can see environment variables for each project also
- ![alt text](image-49.png)
- ![alt text](image-50.png)

### .Net Aspire Service Defaults Project
- We can see the csproj file of this project and also see that IsAspireSharedProject is set to true here
- Also both apiservice and frontend applications have reference to the ServiceDefaults Project
- Both ApiService and Frontend projects also include: builder.ServiceDefaults() extension method which is also defined below.
- This way we dont have to duplicate boilerplate code
- ![alt text](image-51.png)
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsAspireSharedProject>true</IsAspireSharedProject>
  </PropertyGroup>

  <ItemGroup>
    <FrameworkReference Include="Microsoft.AspNetCore.App" />

    <PackageReference Include="Microsoft.Extensions.Http.Resilience" Version="9.0.0" />
    <PackageReference Include="Microsoft.Extensions.ServiceDiscovery" Version="9.0.0" />
    <PackageReference Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" Version="1.9.0" />
    <PackageReference Include="OpenTelemetry.Extensions.Hosting" Version="1.9.0" />
    <PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.9.0" />
    <PackageReference Include="OpenTelemetry.Instrumentation.Http" Version="1.9.0" />
    <PackageReference Include="OpenTelemetry.Instrumentation.Runtime" Version="1.9.0" />
  </ItemGroup>

</Project>

```
- The code for Extensions.cs file is as follows:
```c#
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ServiceDiscovery;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Microsoft.Extensions.Hosting;

// Adds common .NET Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// This project should be referenced by each service project in your solution.
// To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults
public static class Extensions
{
    public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.ConfigureOpenTelemetry();

        builder.AddDefaultHealthChecks();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on resilience by default
            http.AddStandardResilienceHandler();

            // Turn on service discovery by default
            http.AddServiceDiscovery();
        });

        // Uncomment the following to restrict the allowed schemes for service discovery.
        // builder.Services.Configure<ServiceDiscoveryOptions>(options =>
        // {
        //     options.AllowedSchemes = ["https"];
        // });

        return builder;
    }

    public static TBuilder ConfigureOpenTelemetry<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            })
            .WithTracing(tracing =>
            {
                tracing.AddSource(builder.Environment.ApplicationName)
                    .AddAspNetCoreInstrumentation()
                    // Uncomment the following line to enable gRPC instrumentation (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
                    //.AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation();
            });

        builder.AddOpenTelemetryExporters();

        return builder;
    }

    private static TBuilder AddOpenTelemetryExporters<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (useOtlpExporter)
        {
            builder.Services.AddOpenTelemetry().UseOtlpExporter();
        }

        // Uncomment the following lines to enable the Azure Monitor exporter (requires the Azure.Monitor.OpenTelemetry.AspNetCore package)
        //if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
        //{
        //    builder.Services.AddOpenTelemetry()
        //       .UseAzureMonitor();
        //}

        return builder;
    }

    public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddHealthChecks()
            // Add a default liveness check to ensure app is responsive
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        // Adding health checks endpoints to applications in non-development environments has security implications.
        // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
        if (app.Environment.IsDevelopment())
        {
            // All health checks must pass for app to be considered ready to accept traffic after starting
            app.MapHealthChecks("/health");

            // Only health checks tagged with the "live" tag must pass for app to be considered alive
            app.MapHealthChecks("/alive", new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live")
            });
        }

        return app;
    }
}



```
- Note above we also integrate the default endpoint extension method which adds health checks to our microservices application

### Managing inter-service communication with .Net Aspire
- Inside the Frontend Web App, we can see that we add an HttpClient to call our ApiService

```c#
using AspireSample.Web;
using AspireSample.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient<WeatherApiClient>(client =>
    {
        // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
        // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
        client.BaseAddress = new("https+http://apiservice");
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseOutputCache();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();


```
- As you can see we have a named HttpClient called WeatherApiClient which is responsible for fetching the data from the WeatherForecast Api
- Note the base address where we are trying the strongly typed name of the apiservice.

```c#
namespace AspireSample.Web;

public class WeatherApiClient(HttpClient httpClient)
{
    public async Task<WeatherForecast[]> GetWeatherAsync(int maxItems = 10, CancellationToken cancellationToken = default)
    {
        List<WeatherForecast>? forecasts = null;

        await foreach (var forecast in httpClient.GetFromJsonAsAsyncEnumerable<WeatherForecast>("/weatherforecast", cancellationToken))
        {
            if (forecasts?.Count >= maxItems)
            {
                break;
            }
            if (forecast is not null)
            {
                forecasts ??= [];
                forecasts.Add(forecast);
            }
        }

        return forecasts?.ToArray() ?? [];
    }
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

```
## Adding .Net Aspire to Existing .Net Applications
- ![alt text](image-52.png)
- Earlier we used to create microservices with Dockerfiles and docker compose files.
- Inside the docker compose files, we had to setup the environment for each microservice like this
- ![alt text](image-53.png)
- This becomes complex as the solution grows
- .Net Aspire helps us with this
- For this delete the Dockerfiles and docker compose files
- Now add .Net Aspire Orchestrator Support
- ![alt text](image-54.png)
- We can also manually add the Aspire AppHost project and ServiceDefaults project
- ![alt text](image-55.png)
- ![alt text](image-56.png)
- Now we can add the .Net Aspire orchestrator support for both frontend and api projects
- We can add Aspire hosting packages like this
- ![alt text](image-57.png)

## Building EShop project with .Net Aspire
- First we will develop catalog microservice
- ![alt text](image-58.png)
- ![alt text](image-59.png)

## Developing Catalog Microservices with PostgreSQL
- ![alt text](image-60.png)
- ![alt text](image-61.png)
- ![alt text](image-62.png)
- ![alt text](image-63.png)
- ![alt text](image-64.png)
- We will use N-Layer Architecture
- ![alt text](image-65.png)
- ![alt text](image-66.png)
- ![alt text](image-67.png)
- ![alt text](image-68.png)
- When we add an Asp.net core WebApi core in an .Net Aspire supported project, we get the option to enlist in .Net Aspire orchestration
- ![alt text](image-69.png)

## Doing Hosting and Client Integration
- ![alt text](image-70.png)
- Hosting Integrations are all about attaching resources, whereas client integration is all about consuming those resources.
- ![alt text](image-71.png)
- For client integrations for Catalog Project we will do this
- ![alt text](image-72.png)
- We will add the hosting package like this
```xml
    <PackageReference Include="Aspire.Hosting.PostgreSQL" Version="9.3.1" />
```
- Then we will update Program.cs file like this

```c#
var builder = DistributedApplication.CreateBuilder(args);

//Backing Services
var postgres = builder
    .AddPostgres("postgres")
    .WithPgAdmin() //optional UI tool to manage postgres
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent) 
    //Container lifetime is set as persistent so container data is retained across container restarts;

// Provides connection info for microservices
var catalogDb = postgres.AddDatabase("catalogdb");

//Projects
builder
    .AddProject<Projects.Catalog>("catalog")
    .WithReference(catalogDb)
    .WaitFor(catalogDb);

builder.Build().Run();

```
- ![alt text](image-73.png)
- Environment variables for catalogDb are automatically injected for the catalog microservice
- ![alt text](image-74.png)
- Note that we can run pgAdmin tool
- Right now no database is created for catalog db.

### Catalog Client Integrations
- We will add this package to the catalog microservice

```xml
 <ItemGroup>
   <PackageReference Include="Aspire.Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.3.1" />
 </ItemGroup>

```
- We will add the following to Program.cs file for the microservice:

```c#
builder.AddNpgsqlDbContext<ProductDbContext>(connectionName: "catalogdb");

```

### Developing the Domain Model
- We will run the following command to add the migrations
```shell
Add-Migration InitialCreate -OutputDir Data/Migrations -Project Catalog

Update-Database

```
- We will perform auto-migrations in Program.cs file
- We will add the code app.UseMigration() method inside the Program.cs file
- This ensures DB is updated before the microservices run
- We can do this by adding this extension method
```c#
namespace Catalog.Data
{
    public static class Extensions
    {
        public static void UseMigration(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
            //Apply the Migrations to create the database and tables
            dbContext.Database.Migrate();
            DataSeeder.Seed(dbContext);
        }
    }

    public class DataSeeder
    {
        public static void Seed(ProductDbContext context)
        {
            if (!context.Products.Any())
            {
                context.Products.AddRange(
                    new Models.Product
                    {
                        Id = 1,
                        Name = "Product 1",
                        Description = "Description for Product 1",
                        Price = 9.99m,
                        ImageUrl = "https://example.com/product1.jpg"
                    },
                    new Models.Product
                    {
                        Id = 2,
                        Name = "Product 2",
                        Description = "Description for Product 2",
                        Price = 19.99m,
                        ImageUrl = "https://example.com/product2.jpg"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}



```
- Now we will add the ProductService to perform CRUD operations

```c#
namespace Catalog.Services
{
    public class ProductService(ProductDbContext dbContext)
    {

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await dbContext.Products.ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await dbContext.Products.FindAsync(id);
        }
        public async Task CreateProductAsync(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product updatedProduct, Product inputProduct)
        {
            updatedProduct.Name = inputProduct.Name;
            updatedProduct.Description = inputProduct.Description;
            updatedProduct.Price = inputProduct.Price;
            updatedProduct.ImageUrl = inputProduct.ImageUrl;

            dbContext.Products.Update(updatedProduct);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await dbContext.Products.FindAsync(id);
            if (product == null) return;
            dbContext.Products.Remove(product);
            await dbContext.SaveChangesAsync();
        }
    }

}


```
- Now we will build the minimal API endpoints

```c#
namespace Catalog.Endpoints
{
    public static class ProductEndpoints
    {
        public static void MapProductEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/products");
            group.MapGet("/", async (ProductService productService) =>
            {
                var products = await productService.GetProductsAsync();
                return Results.Ok(products);
            })
            .WithName("GetAllProducts")
            .Produces<List<Product>>(StatusCodes.Status200OK);

            group.MapGet("/{id}", async (int id, ProductService productService) =>
            {
                var product = await productService.GetProductByIdAsync(id);
                return product is not null ? Results.Ok(product) : Results.NotFound();
            })
            .WithName("GetProductById")
            .Produces<Product>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);


            group.MapPost("/", async (Product product, ProductService productService) =>
            {
                await productService.CreateProductAsync(product);
                return Results.Created($"/products/{product.Id}", product);
            })
            .WithName("CreateProduct")
            .Produces<Product>(StatusCodes.Status201Created); 


            app.MapPut("/{id}", async (int id, Product inputProduct, ProductService productService) =>
            {
                if (id != inputProduct.Id)
                {
                    return Results.BadRequest();
                }
                var upatedProduct = await productService.GetProductByIdAsync(id);
                await productService.UpdateProductAsync(upatedProduct,inputProduct);
                return Results.NoContent();
            })
            .WithName("UpdateProduct")
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status204NoContent)
            ; 


            app.MapDelete("/{id}", async (int id, ProductService productService) =>
            {
                await productService.DeleteProductAsync(id);
                return Results.NoContent();
            })
            .WithName("DeleteProduct")
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status204NoContent); 
        }
    }
}


```

- We will reference this in Program.cs file of Catalog Microservice

```c#
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<ProductDbContext>(connectionName: "catalogdb");

builder.Services.AddScoped<ProductService>();

var app = builder.Build();
// Configure the HTTP request pipeline.

app.MapDefaultEndpoints();
app.UseMigration();

app.MapProductEndpoints();

app.UseHttpsRedirection();

app.Run();
```
- We will write HTTP test methods inside the Catalog.http file

```shell
@Catalog_HostAddress = http://localhost:7234/products

GET {{Catalog_HostAddress}}/
Accept: application/json

###

GET {{Catalog_HostAddress}}/1
Accept: application/json

###

POST {{Catalog_HostAddress}}/
Content-Type: application/json
{
  "id": 11,
  "name": "New New Product",
  "description": "This is a new product.",
  "price": 19.99,
  "imageUrl": "productnew.png"
}

###
PUT {{Catalog_HostAddress}}/11
Content-Type: application/json
{
  "name": "Updated New Product",
  "description": "This is an updated product.",
  "price": 24.99,
  "imageUrl": "productupdated.png"
}

###
DELETE {{Catalog_HostAddress}}/11
Content-Type: application/json


```

- We will set AppHost as startup project and run the project
- Then we will execute the above http test methods one by one and we can test them out
- ![alt text](image-75.png)
- ![alt text](image-76.png)

## Develop Basket Microservice with Redis
- ![alt text](image-77.png)
- ![alt text](image-78.png)
- ![alt text](image-79.png)
- ![alt text](image-80.png)
- ![alt text](image-81.png)
- ![alt text](image-82.png)
- ![alt text](image-83.png)
- ![alt text](image-84.png)
- ![alt text](image-86.png)

### Creating the Basket Microservice
- We will first all the models

```c#
namespace Basket.Models
{
    public class ShoppingCartItem
    {
        public int Quantity { get; set; } = default!;
        public string ProductId { get; set; } = default!;
        public string Color { get; set; } = default!;

        //comes from the catalog module
        public string ProductName { get; set; } = default!;
        public decimal Price { get; set; } = default!;
    }
}


namespace Basket.Models
{
    public class ShoppingCart
    {
        public string UserName { get; set; } = default!;
        public List<ShoppingCartItem> Items { get; set; } = new ();

        public decimal TotalPrice => Items.Sum(item => item.Price * item.Quantity);
    }
}


```

- Now we will do Hosting Integrations
- ![alt text](image-87.png)
- Our AppHost csproj file is like this
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <Sdk Name="Aspire.AppHost.Sdk" Version="9.0.0" />

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsAspireHost>true</IsAspireHost>
    <UserSecretsId>270c7f5a-bf2c-43e1-8e4c-475e855ffc4c</UserSecretsId>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Aspire.Hosting.AppHost" Version="9.0.0" />
    <PackageReference Include="Aspire.Hosting.PostgreSQL" Version="9.3.1" />
    <PackageReference Include="Aspire.Hosting.Redis" Version="9.3.1" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\Catalog\Catalog.csproj" />
    <ProjectReference Include="..\Basket\Basket.csproj" />
  </ItemGroup>

</Project>


```
- The Program.cs file for AppHost project is like this
```c#
var builder = DistributedApplication.CreateBuilder(args);

//Backing Services
var postgres = builder
    .AddPostgres("postgres")
    .WithPgAdmin()
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var catalogDb = postgres.AddDatabase("catalogdb");


var cache = builder
    .AddRedis("cache")
    .WithRedisInsight() //Used for monitoring and visualizing Redis data
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

//Projects
builder
    .AddProject<Projects.Catalog>("catalog")
    .WithReference(catalogDb)
    .WaitFor(catalogDb);


var basket = builder
    .AddProject<Projects.Basket>("basket")
    .WithReference(cache)
    .WaitFor(cache);

builder.Build().Run();


```

### Doing Client Integration of Redis with Basket Microservice
- We will add reference to Redis
- The csproj file for Basket Microservice will have the following code:
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Aspire.StackExchange.Redis.DistributedCaching" Version="9.3.1" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\ServiceDefaults\ServiceDefaults.csproj" />
  </ItemGroup>

</Project>


```
- We will add the following code in Program.cs file of Basket Microservice:
```c#
builder.AddRedisDistributedCache(connectionName: "cache");

```
- We will now add the BasketService to get/update/delete from the cache
```c#
using Basket.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.Services
{
    public class BasketService(IDistributedCache cache)
    {
        public async Task<ShoppingCart?> GetBasket(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("User name cannot be null or empty.", nameof(userName));
            }
            var basket = await cache.GetStringAsync(userName);
            return string.IsNullOrEmpty(basket) ? null : System.Text.Json.JsonSerializer.Deserialize<ShoppingCart>(basket);
        }

        public async Task UpdateBasket(ShoppingCart basket)
        {
            await cache.SetStringAsync(basket.UserName, System.Text.Json.JsonSerializer.Serialize(basket), 
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
                });
        }

        public async Task DeleteBasket(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("User name cannot be null or empty.", nameof(userName));
            }
            await cache.RemoveAsync(userName);

        }
}


```
- Now we will write the code for BasketEndpoints:
```c#
namespace Basket.Endpoints
{
    public static class BasketEndpoints
    {
        public static void MapBasketEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("basket");

            app.MapGet("/{userName}", async (string userName, BasketService basketService) =>
            {
                var basket = await basketService.GetBasket(userName);
                return basket is not null ? Results.Ok(basket) : Results.NotFound();
            })
            .WithName("GetBasket")
            .Produces<ShoppingCart>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            app.MapPost("/", async (Basket.Models.ShoppingCart basket, BasketService basketService) =>
            {
                await basketService.UpdateBasket(basket);
                return Results.Created("GetBasket", basket);
            })
            .WithName("UpdateBasket")
            .Produces<ShoppingCart>(StatusCodes.Status201Created);


            app.MapDelete("/{userName}", async (string userName, BasketService basketService) =>
            {
                await basketService.DeleteBasket(userName);
                return Results.NoContent();
            })
            .WithName("DeleteBasket")
            .Produces<ShoppingCart>(StatusCodes.Status204NoContent);
        }
    }
}

```
- Now we will need to update Program.cs file
```c#
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.AddServiceDefaults();
builder.AddRedisDistributedCache(connectionName: "cache");
builder.Services.AddScoped<BasketService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapDefaultEndpoints();
app.MapBasketEndpoints();

app.UseHttpsRedirection();

app.Run();
```
- We will update Basket.http to perform testing of the endpoints in Basket Microservice

```shell
@Basket_HostAddress = https://localhost:7055/basket

GET {{Basket_HostAddress}}/swn
Accept: application/json

###

POST {{Basket_HostAddress}}
Content-Type: application/json
{
  "UserName": "swn",
  "Items": [
    {
      "Quantity": 2,
      "Color": "Red",
      "Price": 500
      "ProductId": 1,
      "ProductName": "Solar powered Flashlight"
    },
    {
      "Quantity": 1,
      "Color": "Blue",
      "Price": 500
      "ProductId": 2,
      "ProductName": "Hiking Poles"
    }
  ]
}

###

DELETE {{Basket_HostAddress}}/swn
Content-Type: application/json


```
- ![alt text](image-88.png)
- We also also verify this in Redis Insights
- ![alt text](image-89.png)
- ![alt text](image-90.png)
- ![alt text](image-91.png)

## Sync Communication between Catalog-Basket with .NET Aspire Service Discovery
