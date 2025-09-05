# HippoCamp MemoryStore Database Setup

## Quick Start

### 1. Start PostgreSQL with pgvector
```bash
docker-compose up -d postgres
```

### 2. Verify Database Connection
```bash
# Run the MemoryStore API
dotnet run --project src/HippoCamp.MemoryStore

# Check health endpoint
curl http://localhost:5000/health
```

### 3. Database Connection Details
- **Host**: localhost:5432
- **Database**: hippocampmemorystoredb  
- **Username**: hippocamp
- **Password**: dev-password (development only)

## Vector Extension Verification
The init script automatically:
- Installs the `vector` extension
- Creates a test table with vector(1536) column
- Verifies functionality

## Production Configuration
Set environment variables:
- `POSTGRES_HOST`
- `POSTGRES_PORT` 
- `POSTGRES_DB`
- `POSTGRES_USER`
- `POSTGRES_PASSWORD`

## Health Monitoring
- Health check endpoint: `/health`
- Monitors PostgreSQL connectivity
- Returns 200 OK when database is accessible