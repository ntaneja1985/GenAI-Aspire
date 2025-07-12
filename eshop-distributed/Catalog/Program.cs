


using System.Reflection;
using Microsoft.SemanticKernel;
using ServiceDefaults.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<ProductDbContext>(connectionName: "catalogdb");

builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ProductAIService>();

//Scan the current assembly for consumers, sagas, and state machines and register them with MassTransit
builder.Services.AddMassTransitWithAssemblies(Assembly.GetExecutingAssembly());

builder.AddOllamaSharpChatClient("ollama-llama3-2");
builder.AddOllamaSharpEmbeddingGenerator("ollama-all-minilm");

//Register an in-memory vector store for product vectors
builder.Services.AddInMemoryVectorStoreRecordCollection<int, ProductVector>("products");

var app = builder.Build();



// Configure the HTTP request pipeline.

app.MapDefaultEndpoints();
app.UseMigration();

app.MapProductEndpoints();

app.UseHttpsRedirection();

app.Run();
