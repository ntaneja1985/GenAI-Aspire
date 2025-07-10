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


builder.Build().Run();
