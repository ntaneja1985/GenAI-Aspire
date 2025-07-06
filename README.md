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
