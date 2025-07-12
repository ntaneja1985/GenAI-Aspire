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
