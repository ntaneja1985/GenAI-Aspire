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
- ![alt text](image-92.png)
- We can use WithReference keyword to setup communication between microservices
- ![alt text](image-93.png)
- Basically, when we update the Basket, we need the latest Product Prices
- ![alt text](image-95.png)
- ![alt text](image-96.png)
- So we will first update Program.cs file in AppHost Project
```c#
//Projects
var catalog = builder
    .AddProject<Projects.Catalog>("catalog")
    .WithReference(catalogDb)
    .WaitFor(catalogDb);


var basket = builder
    .AddProject<Projects.Basket>("basket")
    .WithReference(cache)
    .WithReference(catalog) //Add reference to catalog microservice
    .WaitFor(cache);

```
- Then we can see that references to catalog microservice are injected in the basket microservice
- ![alt text](image-97.png)
- ![alt text](image-98.png)
- Now we need to add a CatalogApiClient to the BasketMicroservice which fetch the latest price for a Product
```c#
using Catalog.Models;

namespace Basket.ApiClients
{
    public class CatalogApiClient(HttpClient httpClient)
    {
        public async Task<Product> GetProductByIdAsync(int id)
        {
            var response = await httpClient.GetFromJsonAsync<Product>($"/products/{id}");
            return response ?? throw new Exception($"Product with id {id} not found.");
        }
    }
}


```
- ![alt text](image-99.png)
- Then we also need to register this HttpClient in the Program.cs file of Basket microservice
```c#
builder.Services.AddHttpClient<CatalogApiClient>(client =>
{
    client.BaseAddress = new Uri("https+http://catalog");
});
```
- ![alt text](image-101.png)
- Now we can update BasketService as follows:

```c#

using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.Services
{
    public class BasketService(IDistributedCache cache, CatalogApiClient catalogApiClient)
    {
        public async Task<ShoppingCart?> GetBasket(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("User name cannot be null or empty.", nameof(userName));
            }
            var basket = await cache.GetStringAsync(userName);
            return string.IsNullOrEmpty(basket) ? null : JsonSerializer.Deserialize<ShoppingCart>(basket);
        }

        public async Task UpdateBasket(ShoppingCart basket)
        {
            //Before updating the shopping cart, call the Catalog Microservice's GetProductByIdAsync method
            //Get latest product information and set the Price and ProductName when adding/updating the item into the Shopping Cart

            foreach (var item in basket.Items)
            {
                var product = await catalogApiClient.GetProductByIdAsync(int.Parse(item.ProductId));
                if (product != null)
                {
                    item.Price = product.Price;
                    item.ProductName = product.Name;
                }
            }


            await cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket),
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
}


```
- Now we can test this out as follows. Notice, that from the Basket, we are setting the price as 0 , but when the response comes, it has the price set to 9.99 and 19.99 for products with Id 1 and 2 respectively. This is because of synchronous communication between microservices
- ![alt text](image-102.png)

## Async Communication between Microservices with RabbitMq/MassTransit orchestrated with .NET Aspire
- ![alt text](image-103.png)
- ![alt text](image-104.png)
- ![alt text](image-105.png)
- ![alt text](image-106.png)
- ![alt text](image-107.png)
- A domain event ProductPriceChanged Event will lead to integration event: ProductPriceChanged Integration Event
- ![alt text](image-108.png)
- ![alt text](image-109.png)
- ![alt text](image-110.png)

### RabbitMq Hosting Integration in .NET Aspire
- We will first add the RabbitMq Aspire package in AppHost project
-  Modify the Program.cs file of AppHost as follows:
```c#
var rabbitMq = builder
    .AddRabbitMQ("rabbitmq")
    .WithManagementPlugin() //Enables the RabbitMQ management plugin for monitoring and management
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

//Projects
var catalog = builder
    .AddProject<Projects.Catalog>("catalog")
    .WithReference(catalogDb)
    .WithReference(rabbitMq)
    .WaitFor(catalogDb)
    .WaitFor(rabbitMq);


var basket = builder
    .AddProject<Projects.Basket>("basket")
    .WithReference(cache)
    .WithReference(catalog)
    .WithReference(rabbitMq)
    .WaitFor(cache)
    .WaitFor(rabbitMq);

```
- ![alt text](image-111.png)
- ![alt text](image-112.png)
- The environment variables for rabbitMq are automatically added to basket and catalog microservice
- ![alt text](image-113.png)

### Creating Shared Messaging Folders
- ![alt text](image-114.png)
- ![alt text](image-115.png)
- The following is our Integration Event and ProductPriceChangedIntegration Event Models

```c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDefaults.Messaging.Events
{
    public record IntegrationEvent
    {
        public Guid EventId => Guid.NewGuid();
        public DateTime OccurredOn  => DateTime.UtcNow;

        public string EventType => GetType().AssemblyQualifiedName;
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDefaults.Messaging.Events
{
    public record ProductPriceChangedIntegrationEvent : IntegrationEvent
    {
        public int ProductId { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Price { get; set; } = default!;
        public string ImageUrl { get; set; } = default!;
    }
}


```
- Next we will setup RabbitMq using MassTransit by adding a MassTransitExtensions class in the ServiceDefaults Project as follows:

```c#
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ServiceDefaults.Messaging
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection AddMassTransitWithAssemblies(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddMassTransit(config =>
            {
                // Configure MassTransit with RabbitMQ
                config.SetKebabCaseEndpointNameFormatter();

                // Use in-memory saga repository
                config.SetInMemorySagaRepositoryProvider();

                // Register the consumers
                config.AddConsumers(assemblies);

                // Register the saga state machines
                config.AddSagaStateMachines(assemblies);

                // Register the sagas
                config.AddSagas(assemblies);

                // Register the activities
                config.AddActivities(assemblies);

                config.UsingRabbitMq((context, cfg) =>
                {
                    var configuration = context.GetRequiredService<IConfiguration>();
                    // Get the RabbitMQ connection string from environment variables of the microservice
                    var connectionString = configuration.GetConnectionString("rabbitmq");
                    cfg.Host(connectionString);
                    cfg.ConfigureEndpoints(context);
                });
            });
            // Register the event handlers
            //services.AddScoped<ProductPriceChangedIntegrationEventHandler>();
            return services;
        }
    }
}

```
### Register MassTransit Packages in Catalog and Basket DI in Program.cs file
- ![alt text](image-116.png)
- We will call the AddMassTransit extension method in Basket and Catalog Microservices
- We will go to Program.cs file of each microservice and add the following code:
```c#
//Scan the current assembly for consumers, sagas, and state machines and register them with MassTransit
builder.Services.AddMassTransitWithAssemblies(Assembly.GetExecutingAssembly());
```

### Add .Net Aspire Trace for MassTransit Operations
- We will add telemetry for MassTransit
- Add it in Extensions class of ServiceDefaults project
- This will allow us to see trace logs for MassTransit operations
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

namespace Microsoft.Extensions.Hosting
{
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
                        .AddHttpClientInstrumentation()
                        //Add tracing for MassTransit
                        .AddSource("MassTransit");
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
}

```

### Catalog Microservice Publish ProductPriceChanged Integration Event
- ![alt text](image-117.png)
- Inject the IBus interface in ProductService class and update the code for update product method
```c#
public async Task UpdateProductAsync(Product updatedProduct, Product inputProduct)
{
    // if price has changed, raise ProductPriceChanged Integration Event
    if (updatedProduct.Price != inputProduct.Price)
    {
        //Publish product price changed integration event for update basket prices
        var integrationEvent = new ProductPriceChangedIntegrationEvent
        {
            ProductId = updatedProduct.Id,
            Name = inputProduct.Name,
            Description = inputProduct.Description,
            Price = inputProduct.Price, //set updated price
            ImageUrl = inputProduct.ImageUrl
        };

        await bus.Publish(integrationEvent);

    }

    //update product with new values
    updatedProduct.Name = inputProduct.Name;
    updatedProduct.Description = inputProduct.Description;
    updatedProduct.Price = inputProduct.Price;
    updatedProduct.ImageUrl = inputProduct.ImageUrl;

    dbContext.Products.Update(updatedProduct);
    await dbContext.SaveChangesAsync();
}

```
- However we can face dual write problem in the above setup, what if the event is published but the database save operation failed ? 
- Conversely what if the event is not published correctly
- To solve this we can use Outbox pattern or Saga Pattern to ensure these write operations are atomic. 

### Basket Subscribe and Consume ProductPriceChanged Integration Event
- In the Basket Microservice, create a ProductPriceChangedIntegrationEvent handler
```c#
using MassTransit;
using ServiceDefaults.Messaging.Events;

namespace Basket.EventHandlers
{
    public class ProductPriceChangedIntegrationEventHandler(BasketService basketService) : IConsumer<ProductPriceChangedIntegrationEvent>
    {
        public async Task Consume(ConsumeContext<ProductPriceChangedIntegrationEvent> context)
        {
            // find products on basket and update price
            await basketService.UpdateBasketItemProductPrices(context.Message.ProductId, context.Message.Price);
        }
    }
}

```
- The code for updateBasketItemProductPrices is as follows:
```c#
 internal async Task UpdateBasketItemProductPrices(int productId, decimal price)
 {
    //IDistributedCache doesnot support list of keys function

     var basket = await GetBasket("swn1");
     var item = basket!.Items.FirstOrDefault(x =>x.ProductId==productId.ToString());
     if (item != null)
     {
         item.Price = price;
         await cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket));
     }
 }
```

- Please note IDistributedCache doesnot support listing of keys
- I tried to update product prices for all the baskets for all the username keys, but was not able to do so as listing of keys is not supported
- ![alt text](image-118.png)
- ![alt text](image-119.png)
- ![alt text](image-120.png)

## Secure Basket with Keycloak Authentication orchestrate .Net Aspire
- ![alt text](image-121.png)
- ![alt text](image-122.png)
- We will setup Keycloak into .NET Aspire for Identity Provider
- We will create Realm(Tenants), User and Client for OpenID Connect with Keycloak Identity
- We will use JwtBearer token for OpenIdConnect with Keycloak Identity
- We will get current user from token
- ![alt text](image-123.png)
- ![alt text](image-124.png)

### Keycloak Hosting Integration in .Net Aspire
- ![alt text](image-125.png)
- We will setup the keycloak container
- Add the Aspire.Hosting.Keycloak pre-release package in AppHost project
- Add the following code in Program.cs
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



var rabbitMq = builder
    .AddRabbitMQ("rabbitmq")
    .WithManagementPlugin() //Enables the RabbitMQ management plugin for monitoring and management
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var keycloak = builder
    .AddKeycloak("keycloak", 8080)
    .WithDataVolume() //Persist Keycloak data across restarts
    .WithLifetime(ContainerLifetime.Persistent);
    //.WithAdminUser("admin", "admin") //Default admin user credentials
    //.WithRealm("master") //Default realm for Keycloak
    //.WithRealm("catalog"); //Custom realm for the catalog service

//Projects
var catalog = builder
    .AddProject<Projects.Catalog>("catalog")
    .WithReference(catalogDb)
    .WithReference(rabbitMq)
    .WaitFor(catalogDb)
    .WaitFor(rabbitMq);


var basket = builder
    .AddProject<Projects.Basket>("basket")
    .WithReference(cache)
    .WithReference(catalog)
    .WithReference(rabbitMq)
    .WithReference(keycloak)
    .WaitFor(cache)
    .WaitFor(rabbitMq)
    .WaitFor(keycloak);


builder.Build().Run();

```
- Once the project is up and running, we can login to Keycloak management console:
- ![alt text](image-126.png)
- ![alt text](image-127.png)

### Creating Realm, User and Client for OpenIdConnect with Keycloak Identity Provider
- ![alt text](image-128.png)
- ![alt text](image-129.png)
- ![alt text](image-130.png)
- ![alt text](image-131.png)
- ![alt text](image-132.png)
- ![alt text](image-133.png)
- We can get the token using the endpoint for keycloak
- We will send the following request from Basket.http
```shell
POST http://localhost:8080/realms/eshop/protocol/openid-connect/token
Content-Type: application/x-www-form-urlencoded

grant_type=password&client_id=eshop-client&scope=email openid&username=test&password=p@ssw0rd

```
- This will give us an access token
- ![alt text](image-134.png)
- We will add the Aspire.Keycloak.Authentication library to Basket Microservice
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <Compile Include="..\Catalog\Models\Product.cs" Link="Models\Product.cs" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Aspire.Keycloak.Authentication" Version="9.3.1-preview.1.25305.6" />
    <PackageReference Include="Aspire.StackExchange.Redis.DistributedCaching" Version="9.3.1" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\ServiceDefaults\ServiceDefaults.csproj" />
  </ItemGroup>

</Project>

```
- Now we need to register keycloak authentication in Basket Microservice's Program.cs file
```c#

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

```
- Now we will ensure that all methods in BasketEndpoints require authorization. This will be updated as follows:
```c#
 public static class BasketEndpoints
 {
     public static void MapBasketEndpoints(this IEndpointRouteBuilder app)
     {
         var group = app.MapGroup("basket");

         group.MapGet("/{userName}", async (string userName, BasketService basketService) =>
         {
             var basket = await basketService.GetBasket(userName);
             return basket is not null ? Results.Ok(basket) : Results.NotFound();
         })
         .WithName("GetBasket")
         .Produces<ShoppingCart>(StatusCodes.Status200OK)
         .Produces(StatusCodes.Status404NotFound)
         .RequireAuthorization(); // Ensure the user is authenticated

         group.MapPost("/", async (ShoppingCart basket, BasketService basketService) =>
         {
             await basketService.UpdateBasket(basket);
             return Results.Created("GetBasket", basket);
         })
         .WithName("UpdateBasket")
         .Produces<ShoppingCart>(StatusCodes.Status201Created)
         .RequireAuthorization(); // Ensure the user is authenticated


         group.MapDelete("/{userName}", async (string userName, BasketService basketService) =>
         {
             await basketService.DeleteBasket(userName);
             return Results.NoContent();
         })
         .WithName("DeleteBasket")
         .Produces<ShoppingCart>(StatusCodes.Status204NoContent)
         .RequireAuthorization(); // Ensure the user is authenticated
     }
 }

```
- ![alt text](image-135.png)
- We will modify the Basket.http file to include the Authorization header with the Bearer token as follows:
```shell
@Basket_HostAddress = https://localhost:7282/basket
@accessToken = eyJhbGciOiJSUzI1Ni...

GET {{Basket_HostAddress}}/swn1
Accept: application/json
Authorization: Bearer {{accessToken}}

###

POST {{Basket_HostAddress}}
Content-Type: application/json
Authorization: Bearer {{accessToken}}


{
  "UserName": "swn1",
  "Items": [
    {
      "Quantity": 2,
      "Color": "Red",
      "Price": 0,
      "ProductId": "1",
      "ProductName": "Solar powered Flashlight"
    },
    {
      "Quantity": 1,
      "Color": "Blue",
      "Price":0,
      "ProductId": "2",
      "ProductName": "Hiking Poles"
    }
  ]
}

###

DELETE {{Basket_HostAddress}}/swn
Content-Type: application/json
Authorization: Bearer {{accessToken}}


###

POST http://localhost:8080/realms/eshop/protocol/openid-connect/token
Content-Type: application/x-www-form-urlencoded

grant_type=password&client_id=eshop-client&scope=email openid&username=test&password=p@ssw0rd
```

- Now if we send the request will a random access token we will get 401 unauthorized
- ![alt text](image-136.png)
- If we specify the correct access token, we get response as follows:
- ![alt text](image-137.png)

## Developing the Client Application in Blazor
- ![alt text](image-138.png)
- ![alt text](image-139.png)
- ![alt text](image-140.png)
- Add the following code to AppHost project's Program.cs file
```c#
var webapp = builder
    .AddProject<Projects.WebApp>("webapp")
    .WithExternalHttpEndpoints()
    .WithReference(catalog)
    .WithReference(basket)
    .WaitFor(catalog)
    .WaitFor(basket);
```

### Integration of WebApp with Catalog Microservice using CatalogApiClient
- We will add a link to the Product model inside the WebApp
- We will create a new folder called ApiClients in WebApp and create a new CatalogApiClient.cs file as follows
```c#
using Catalog.Models;

namespace WebApp.ApiClients
{
    public class CatalogApiClient(HttpClient httpClient)
    {
        public async Task<List<Product>> GetProducts()
        {
            var response = await httpClient.GetFromJsonAsync<List<Product>>($"/products");
            return response;
        }

        public async Task<Product> GetProductById(int id)
        {
            var response = await httpClient.GetFromJsonAsync<Product>($"/products/{id}");
            return response;
        }
    }
}


```

### Register CatalogApiClient with Aspire Integrations for Service Discovery
- ![alt text](image-141.png)
- We will add the following code to WebApp's Program.cs file
```c#
builder.Services.AddHttpClient<CatalogApiClient>(client =>
{     // Configure the base address for the Catalog API client
    client.BaseAddress = new Uri("https+http://catalog");
});
```
- Next we will add a Products.razor Page
```c#
@page "/products"
@using Catalog.Models
@using Microsoft.AspNetCore.OutputCaching
@using WebApp.ApiClients
@inject CatalogApiClient CatalogApiClient
@attribute [StreamRendering(true)]
@* @attribute [OutputCache(Duration = 5)] *@

<PageTitle>Products</PageTitle>

<h1>Products</h1>

<p>Here are some of our amazing outdoor products that you can purchase.</p>

@if (products == null)
{
    <p><em>Loading...</em></p>
}
else if (products.Count == 0)
{
    <p><em>There is a problem loading our products. Please try again later.</em></p>
}
else
{
    <table class="table">
        <thead>
            <tr>
                <th>Image</th>
                <th>Name</th>
                <th>Description</th>
                <th>Price</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var product in products)
            {
                <tr>
                    <!-- Simulating images being hosted on a CDN -->
                    <td><img height="80" width="80" src="https://raw.githubusercontent.com/MicrosoftDocs/mslearn-dotnet-cloudnative/main/dotnet-docker/Products/wwwroot/images/@product.ImageUrl" /></td>
                    <td>@product.Name</td>
                    <td>@product.Description</td>
                    <td>@product.Price.ToString("C2")</td>
                </tr>
            }
        </tbody>
    </table>
}

@code {
    private List<Product>? products;

    protected override async Task OnInitializedAsync()
    {
        // Simulate asynchronous loading to demonstrate streaming rendering
        products = await CatalogApiClient.GetProducts();
    }
}

```
- Imports.razor acts like a globalUsings and we can apply these references to all pages
```c#
@using System.Net.Http
@using System.Net.Http.Json
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using static Microsoft.AspNetCore.Components.Web.RenderMode
@using Microsoft.AspNetCore.Components.Web.Virtualization
@using Microsoft.JSInterop
@using WebApp
@using WebApp.Components
@using Catalog.Models
@using Microsoft.AspNetCore.OutputCaching
@using WebApp.ApiClients

```
- We can run the application as follows:
- ![alt text](image-142.png)
- ![alt text](image-143.png)

## Output Caching fo Products Page
- ![alt text](image-144.png)
- ![alt text](image-145.png)
- We will add the Aspire Redis Output Caching library to client application i.e WebApp
- ![alt text](image-146.png)
- Add Redis output caching in Program.cs file as follows:
```c#
using WebApp.ApiClients;
using WebApp.Components;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.

builder.AddServiceDefaults();

builder.Services.AddHttpClient<CatalogApiClient>(client =>
{     // Configure the base address for the Catalog API client
    client.BaseAddress = new Uri("https+http://catalog");
});

//Add output caching
builder.AddRedisOutputCache("cache");

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapDefaultEndpoints();

//use output cache
app.UseOutputCache();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

```
- To Products.razor page add the following code:
```c#
@attribute [OutputCache(Duration = 5)]
```

- We can see that if we try to get the Products page within 5 seconds, it will fetch from the cache rather than making a call to the API
- ![alt text](image-147.png)

## Deploy Project to Azure Container Apps
- ![alt text](image-148.png)
- ![alt text](image-149.png)
- ![alt text](image-150.png)
- Azure Container Apps in Microsoft's managed container hosting for microservices
- ![alt text](image-151.png)
- ![alt text](image-152.png)
- We can easily to CI/CD pipelines using azd commands
- ![alt text](image-153.png)
- ![alt text](image-154.png)
- Data Volumes dont work in Azure Container Apps
- We use Data volume to persist data across container restarts
- ![alt text](image-155.png)
- ![alt text](image-156.png)
- ![alt text](image-157.png)
- We will have to make changes to Program.cs of AppHost project as follows and comment out DataVolumes
```c#
var builder = DistributedApplication.CreateBuilder(args);

//Backing Services
var postgres = builder
    .AddPostgres("postgres")
    .WithPgAdmin()
    //.WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);



var catalogDb = postgres.AddDatabase("catalogdb");


var cache = builder
    .AddRedis("cache")
    .WithRedisInsight() //Used for monitoring and visualizing Redis data
    //.WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);



var rabbitMq = builder
    .AddRabbitMQ("rabbitmq")
    .WithManagementPlugin() //Enables the RabbitMQ management plugin for monitoring and management
    //.WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var keycloak = builder
    .AddKeycloak("keycloak", 8080)
    //.WithDataVolume() //Persist Keycloak data across restarts
    .WithLifetime(ContainerLifetime.Persistent);
    //.WithAdminUser("admin", "admin") //Default admin user credentials
    //.WithRealm("master") //Default realm for Keycloak
    //.WithRealm("catalog"); //Custom realm for the catalog service

//Projects
var catalog = builder
    .AddProject<Projects.Catalog>("catalog")
    .WithReference(catalogDb)
    .WithReference(rabbitMq)
    .WaitFor(catalogDb)
    .WaitFor(rabbitMq);


var basket = builder
    .AddProject<Projects.Basket>("basket")
    .WithReference(cache)
    .WithReference(catalog)
    .WithReference(rabbitMq)
    .WithReference(keycloak)
    .WaitFor(cache)
    .WaitFor(rabbitMq)
    .WaitFor(keycloak);

var webapp = builder
    .AddProject<Projects.WebApp>("webapp")
    .WithExternalHttpEndpoints()
    .WithReference(catalog)
    .WithReference(basket)
    .WithReference(cache)
    .WaitFor(catalog)
    .WaitFor(basket);

if (builder.ExecutionContext.IsRunMode)
{
    //Data volumes dont work on Azure Container Apps, so only add while running locally
    postgres.WithDataVolume(); //Persist Postgres data across restarts
    cache.WithDataVolume(); //Persist Redis data across restarts
    keycloak.WithDataVolume(); //Persist Keycloak data across restarts
    rabbitMq.WithDataVolume(); //Persist RabbitMQ data across restarts

}

builder.Build().Run();


```

### Deploy .NET Aspire App with azd commands to ACA
- ![alt text](image-161.png)
- ![alt text](image-160.png)
- ![alt text](image-162.png)
- ![alt text](image-163.png)
- ![alt text](image-164.png)
- ![alt text](image-165.png)
- ![alt text](image-166.png)
- ![alt text](image-167.png)
- ![alt text](image-168.png)
- ![alt text](image-170.png)
- ![alt text](image-171.png)
- Note the webapplication url doesnot include the internal part in the URL
- ![alt text](image-172.png)
- ![alt text](image-173.png)
- ![alt text](image-174.png)
- ![alt text](image-175.png)
- We can view the aspire dashboard also
- ![alt text](image-176.png)
- We can tear down our resources with azd down command
- ![alt text](image-177.png)
- ![alt text](image-178.png)

## .Net Gen AI with Microsoft.Extensions.AI for ChatAI and Semantic Search
- ![alt text](image-179.png)
- ![alt text](image-180.png)
- ![alt text](image-181.png)
- ![alt text](image-182.png)
- ![alt text](image-183.png)
- ![alt text](image-184.png)
- ![alt text](image-185.png)
- ![alt text](image-186.png)
- ![alt text](image-187.png)
- ![alt text](image-188.png)
- ![alt text](image-189.png)

### Ollama and Llama Model Integrations
- ![alt text](image-190.png)
- We will install the following inside the AppHost Project
- ![alt text](image-191.png)
- Add the following code to Program.cs file in AppHost Project
```c#
//AI Services
var ollama = builder
    .AddOllama("ollama", 11434) //Run Ollama on port 11434
    .WithDataVolume() //Persist Ollama data across restarts
    .WithLifetime(ContainerLifetime.Persistent)
    .WithOpenWebUI();

//Add models to Ollama
var llama = ollama.AddModel("llama3.2");

//Projects
var catalog = builder
    .AddProject<Projects.Catalog>("catalog")
    .WithReference(catalogDb)
    .WithReference(rabbitMq)
    .WithReference(llama) //Reference Ollama for AI capabilities
    .WaitFor(catalogDb)
    .WaitFor(rabbitMq)
    .WaitFor(llama);
```
- ![alt text](image-192.png)
- ![alt text](image-193.png)
- ![alt text](image-194.png)

### Ollama Client Integration in Catalog Microservice in .NET Aspire
- We will install Ollama sharp inside the Catalog Microservice
- ![alt text](image-195.png)

### Using the Microsoft.Extensions.AI library
- ![alt text](image-196.png)
- It is an abstraction over the various AI backing services like Azure OpenAI, OpenAI or even Ollama
- We will work with higher level interfaces like IChatClient, ChatMessage etc
- We can even change our provider at a later stage
- ![alt text](image-197.png)
- ![alt text](image-198.png)

### Register OllamaSharpChatClient with Microsoft.Extensions.AI
- ![alt text](image-199.png)
- ![alt text](image-200.png)
- Add the following code in Program.cs file of Catalog Microservice
```c#
builder.AddOllamaSharpChatClient("ollama-llama3-2");
```
### Develop ProductAIService.cs class for Business Layer - Customer Support ChatAI
- Add a ProductAIService class in Catalog Microservice as follows:
```c#
using Microsoft.Extensions.AI;

namespace Catalog.Services
{
    public class ProductAIService
    {
        private readonly IChatClient _chatClient;

        public ProductAIService(IChatClient chatClient)
        {
            _chatClient = chatClient;
        }

        public async Task<string> SupportAsync(string query)
        {
            var systemPrompt = """
                    You are a helpful assistant for a product catalog.
                    You will answer questions about products, their features, and availability.
                    If you do not know the answer, say "I don't know".
                    At the end of your response, include a link to the product page.
                    If the product is not available, say "This product is not available at the moment".
                    Always provide a friendly and helpful response.
                    Offer one of the our products: Hiking Poles-$24.99, Hiking Boots-$89.99, Camping Tent-$199.99.
                    """;

            var chatHistory = new List<ChatMessage>
                {
                    new ChatMessage(ChatRole.System, systemPrompt),
                    new ChatMessage(ChatRole.User, query)
                };

            var resultPrompt = await _chatClient.GetResponseAsync(chatHistory);
            return resultPrompt.Messages[0].ToString();
        }
    }
}


```
- Next register it in Program.cs as follows:
```c#
builder.Services.AddScoped<ProductAIService>();

```
- Now we need to add an endpoint for Support
```c#
  // Support AI
  group.MapGet("/support/${query}", async (string query, ProductAIService productAIService) =>
  {
      if (string.IsNullOrWhiteSpace(query))
      {
          return Results.BadRequest("Query cannot be empty.");
      }
      var response = await productAIService.SupportAsync(query);
      return Results.Ok(response);
  })
  .WithName("ProductSupport")
  .Produces(StatusCodes.Status200OK);

```

- For testing, add the following code to Catalog.http file:
```shell
### Support AI

GET {{Catalog_HostAddress}}/support/give-me-1-outdoor-activity
Accept: application/json


```
- ![alt text](image-201.png)
- ![alt text](image-202.png)

### Blazor FrontEnd Support Page Development
- Add the following code to CatalogApiClient
```c#
   public async Task<string> SupportProducts(string query)
   {
       var response = await httpClient.GetFromJsonAsync<string>($"/products/support/{query}");
       return response;
   }

```
- Add the Support Razor Page as follows:
```c#
@page "/support"

@attribute [StreamRendering(true)]
@rendermode InteractiveServer

@inject CatalogApiClient CatalogApiClient

<PageTitle>Support</PageTitle>

<h1>Support</h1>

<p>Ask questions about our amazing outdoor products that you can purchase.</p>

<div class="form-group">
    <label for="query" class="form-label">Type your question:</label>
    <div class="input-group mb-3">
        <input type="text" id="query" class="form-control" @bind="queryTerm" placeholder="Enter your query..." />
        <button id="btnSend" class="btn btn-primary" @onclick="DoSend" type="submit">Send</button>
    </div>
    <hr />
</div>

@if (response != null)
{
    <p><em>@response</em></p>
}

@code {

    private string queryTerm = default!;
    private string response = default!;

    private async Task DoSend(MouseEventArgs e)
    {
        response = "Loading..";
        await Task.Delay(500);
        response = await CatalogApiClient.SupportProducts(queryTerm);
    }
}

```
- Add a link to Support Page in NavMenu
- ![alt text](image-203.png)
- ![alt text](image-204.png)
- ![alt text](image-205.png)

### Semantic Product Search with Vector Embeddings and VectorDB
- ![alt text](image-206.png)
- ![alt text](image-207.png)
- ![alt text](image-208.png)
- ![alt text](image-209.png)
- ![alt text](image-210.png)
- ![alt text](image-211.png)
- ![alt text](image-212.png)
- ![alt text](image-213.png)
- ![alt text](image-214.png)
- If 2 sentences have similar meaning, their embeddings will be close in a high dimensional space
- ![alt text](image-215.png)


### Hosting Integration for Ollama all-minilm embeddings model
- ![alt text](image-216.png)
- ![alt text](image-217.png)
- Add the following code to add the all-minilm Ollama Embeddings model
```c#
//Add a smaller model for embedding, useful for tasks like semantic search or text classification
var embedding = ollama.AddModel("all-minilm");

//Projects
var catalog = builder
    .AddProject<Projects.Catalog>("catalog")
    .WithReference(catalogDb)
    .WithReference(rabbitMq)
    .WithReference(llama) //Reference Ollama for AI capabilities
    .WithReference(embedding) //Reference embedding model for semantic tasks
    .WaitFor(catalogDb)
    .WaitFor(rabbitMq)
    .WaitFor(llama)
    .WaitFor(embedding);


```

### Client Integration Packages with SemanticKernel and Extensions.VectorData
- ![alt text](image-218.png)
- We need the following packages
- ![alt text](image-219.png)
- ![alt text](image-220.png)
- Add these packages as well
- ![alt text](image-221.png)
- Verify the csproj file of Catalog Microservice as follows:
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Aspire.Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.3.1" />
    <PackageReference Include="CommunityToolkit.Aspire.OllamaSharp" Version="9.6.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.7">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.Extensions.VectorData.Abstractions" Version="9.7.0" />
    <PackageReference Include="Microsoft.SemanticKernel.Connectors.InMemory" Version="1.60.0-preview" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\ServiceDefaults\ServiceDefaults.csproj" />
  </ItemGroup>

</Project>


```

### Develop ProductVector Domain Entity for storing Vector Data
- ![alt text](image-222.png)
- We will create a ProductVector class as follows:
```c#
using Microsoft.Extensions.VectorData;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.Models;

public class ProductVector
{
    [VectorStoreRecordKey]
    public int Id { get; set; }
    [VectorStoreRecordData]
    public string Name { get; set; } = default!;
    [VectorStoreRecordData]
    public string Description { get; set; } = default!;
    [VectorStoreRecordData]
    public decimal Price { get; set; }
    [VectorStoreRecordData]
    public string ImageUrl { get; set; } = default!;

    [NotMapped]
    [VectorStoreRecordVector(384, DistanceFunction.CosineSimilarity)]
    public ReadOnlyMemory<float> Vector { get; set; }
}

```

### Traditional Search in ProductService.cs Business Class
- This is how we traditionally do search using LINQ
- This is best for power users who know the actual product names
```c#
 public async Task<IEnumerable<Product>> SearchProductsAsync(string query)
 {
     return await dbContext.Products
         .Where(p=>p.Name.Contains(query))
         .ToListAsync();
 }

```
- Add this to ProductAIService.cs file:
```c#
 public async Task<IEnumerable<Product>> SearchProductsAsync(string query)
 {
     //use Embedding Generator to convert query to embedding
     //use In-Memory Vector Store
     //Provide Semantic Search
 }

```

### "Microsoft.SemanticKernel.Connectors.InMemory
- This package is all about in-memory vector storage for Semantic Kernel, which is perfect for fast prototyping and lightweight AI scenarios
- Microsoft.SemanticKernel.Connectors.InMemory is a vector store connector that stores embeddings and semantic data directly in memory—no external database required.
- Flat index for fast lookups
- Supports multiple distance functions: CosineSimilarity, DotProductSimilarity, EuclideanDistance, etc.
- Can store multiple vectors per record
- Works with any comparable key and flexible data types
- Fully supports filtering, tagging, and indexing
- Use Cases
- Building a local RAG (Retrieval-Augmented Generation) prototype
- Embedding-based search in small apps
- Temporary memory for chat agents or assistants

### Register Embedding Generator and VectorStore Services in Catalog/Program.cs file
- Our embedding model is all-minilm
- ![alt text](image-223.png)
- Add the following to Program.cs for Catalog Microservice
```c#
builder.AddOllamaSharpEmbeddingGenerator("ollama-all-minilm");

//Register an in-memory vector store
builder.Services.AddInMemoryVectorStoreRecordCollection<int, ProductVector>("products");
```

### Semantic Search Implementation
- To do the semantic search we will follow the following steps
- Convert all products to their vector embeddings and store in the in memory collection vector store
- Convert the query to an embedding
- Perform a vector search
- Map the results to Products object and return the result
- Here is the code of ProductAIService.cs file

```c#
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;

namespace Catalog.Services
{
    public class ProductAIService(ProductDbContext dbContext, 
        IChatClient _chatClient
        , IEmbeddingGenerator<string,Embedding<float>> embeddingGenerator
        , IVectorStoreRecordCollection<int, ProductVector> productVectorCollection)
      
    {
        public async Task<string> SupportAsync(string query)
        {
            var systemPrompt = """
                    You are a helpful assistant for a product catalog.
                    You will answer questions about products, their features, and availability.
                    If you do not know the answer, say "I don't know".
                    At the end of your response, include a link to the product page.
                    If the product is not available, say "This product is not available at the moment".
                    Always provide a friendly and helpful response.
                    Offer one of the our products: Hiking Poles-$24.99, Hiking Boots-$89.99, Camping Tent-$199.99.
                    """;

            var chatHistory = new List<ChatMessage>
                {
                    new ChatMessage(ChatRole.System, systemPrompt),
                    new ChatMessage(ChatRole.User, query)
                };

            var resultPrompt = await _chatClient.GetResponseAsync(chatHistory);
            return resultPrompt.Messages[0].ToString();
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string query)
        {

            //Find all the products, generate the vectors for each product and store them in the vector store
            if (!await productVectorCollection.CollectionExistsAsync())
            {
                await InitEmbeddingsAsync();
            }

            //Generate the vector for the query
            var queryEmbedding = await embeddingGenerator.GenerateVectorAsync(query);


            //Specify the vector search options
            var vectorSearchOptions = new VectorSearchOptions
            {
                Top = 1,
                VectorPropertyName = "Vector"
            };

            //Perform the vectorized search
            var results =
                await productVectorCollection.VectorizedSearchAsync(queryEmbedding, vectorSearchOptions);

            //Map the results to Product objects
            List<Product> products = [];
            await foreach (var resultItem in results.Results)
            {
                products.Add(new Product
                {
                    Id = resultItem.Record.Id,
                    Name = resultItem.Record.Name,
                    Description = resultItem.Record.Description,
                    Price = resultItem.Record.Price,
                    ImageUrl = resultItem.Record.ImageUrl
                });
            }

            return products;
        }

            //Read the products from the database, generate the embeddings and store them in the vector store
        private async Task InitEmbeddingsAsync()
        {
            await productVectorCollection.CreateCollectionIfNotExistsAsync();

            var products = await dbContext.Products.ToListAsync();
            foreach (var product in products)
            {
                var productInfo = $"[{product.Name}] is a product that costs [{product.Price}] and is described as [{product.Description}]";

                var productVector = new ProductVector
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl,
                    Vector = await embeddingGenerator.GenerateVectorAsync(productInfo)
                };

                await productVectorCollection.UpsertAsync(productVector);
            }
        }
    }
}


```
### Develop Search Endpoints in ProductsEndpoint.cs for Semantic Search
- Add the following code in ProductsEndpoint.cs file
```c#

        // Traditional Search
        group.MapGet("search/{query}", async (string query, ProductService service) =>
        {
            var products = await service.SearchProductsAsync(query);

            return Results.Ok(products);
        })
        .WithName("SearchProducts")
        .Produces<List<Product>>(StatusCodes.Status200OK);

        // AI Search
        group.MapGet("aisearch/{query}", async (string query, ProductAIService service) =>
        {
            var products = await service.SearchProductsAsync(query);

            return Results.Ok(products);
        })
        .WithName("AISearchProducts")
        .Produces<List<Product>>(StatusCodes.Status200OK);

```

- Add the following to Catalog.http file
```shell
### Traditional Search

GET {{Catalog_HostAddress}}/search/Hiking
Accept: application/json

### AI Search

GET {{Catalog_HostAddress}}/aisearch/Something_for_rainy_days
Accept: application/json
```
- ![alt text](image-224.png)
- ![alt text](image-225.png)

### Updating the Blazor App for Search
- Update the CatalogApiClient.cs file as follows:
```c#
using Catalog.Models;

namespace WebApp.ApiClients
{
    public class CatalogApiClient(HttpClient httpClient)
    {
        public async Task<List<Product>> GetProducts()
        {
            var response = await httpClient.GetFromJsonAsync<List<Product>>($"/products");
            return response;
        }

        public async Task<Product> GetProductById(int id)
        {
            var response = await httpClient.GetFromJsonAsync<Product>($"/products/{id}");
            return response;
        }

        public async Task<string> SupportProducts(string query)
        {
            var response = await httpClient.GetFromJsonAsync<string>($"/products/support/{query}");
            return response;
        }

        public async Task<List<Product>?> SearchProducts(string query, bool aiSearch)
        {
            if (aiSearch)
            {
                return await httpClient.GetFromJsonAsync<List<Product>>($"/products/aisearch/{query}");
            }
            else
            {
                return await httpClient.GetFromJsonAsync<List<Product>>($"/products/search/{query}");
            }
        }
    }
}

```
- For the UI add the following code to Search.razor page:
```c#
@page "/search"

@attribute [StreamRendering(true)]
@rendermode InteractiveServer

@inject CatalogApiClient CatalogApiClient

<PageTitle>Search Products</PageTitle>

<h1>Search Products</h1>

<p>Search our amazing outdoor products that you can purchase.</p>

<div class="form-group">
    <label for="search" class="form-label">Type your question:</label>
    <div class="input-group mb-3">
        <input type="text" id="search" class="form-control" @bind="searchTerm" placeholder="Enter search term..." />
        <button id="btnSearch" class="btn btn-primary" @onclick="DoSearch" type="submit">Search</button>
    </div>
    <div class="form-check form-switch mb-3">
        <InputCheckbox id="aiSearchCheckBox" @bind-Value="aiSearch" />
        <label class="form-check-label" for="aiSearch">Use Semantic Search</label>
    </div>
    <hr />
</div>

@if (products == null)
{
    <p><em>Loading...</em></p>
}
else if (products.Count == 0)
{
    <p><em>No product found.</em></p>
}
else
{
    <table class="table">
        <thead>
            <tr>
                <th>Image</th>
                <th>Name</th>
                <th>Description</th>
                <th>Price</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var product in products)
            {
                <tr>
                    <!-- Simulating images being hosted on a CDN -->
                    <td><img height="80" width="80" src="https://raw.githubusercontent.com/MicrosoftDocs/mslearn-dotnet-cloudnative/main/dotnet-docker/Products/wwwroot/images/@product.ImageUrl" /></td>
                    <td>@product.Name</td>
                    <td>@product.Description</td>
                    <td>@product.Price.ToString("C2")</td>
                </tr>
            }
        </tbody>
    </table>
}

@code {

    private string searchTerm = default!;
    private bool aiSearch = false;
    private List<Product>? products = [];

    private async Task DoSearch(MouseEventArgs e)
    {
        await Task.Delay(500);
        products = await CatalogApiClient.SearchProducts(searchTerm, aiSearch);
    }
}

```
- ![alt text](image-226.png)
- ![alt text](image-227.png)
- ![alt text](image-228.png)
- ![alt text](image-229.png)
- ![alt text](image-230.png)

