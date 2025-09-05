var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL with pgvector extension support
var postgres = builder.AddPostgres("postgresql", port: 5432)
    .WithImage("pgvector/pgvector", "pg16")
    .WithEnvironment("POSTGRES_PASSWORD", "dev-password")
    .WithEnvironment("POSTGRES_DB", "hippocampmemorystoredb")
    .WithEnvironment("POSTGRES_USER", "hippocamp");

// Create the memory store database with vector extension support
var memoryStoreDb = postgres.AddDatabase("memorystore");

// Add the MemoryStore service with reference to the database
var memoryStore = builder.AddProject<Projects.HippoCamp_MemoryStore>("memorystore-api")
    .WithReference(memoryStoreDb)
    .WaitFor(postgres);

builder.Build().Run();