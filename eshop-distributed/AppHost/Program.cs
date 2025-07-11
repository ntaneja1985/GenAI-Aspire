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
