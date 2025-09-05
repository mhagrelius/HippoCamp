---
name: memory-store
status: backlog
created: 2025-09-05T00:40:42Z
progress: 0%
prd: .claude/prds/memory-store.md
github: https://github.com/mhagrelius/HippoCamp/issues/6
---

# Epic: Memory Store

## Overview
Build a standalone microservice for AI agent long-term memory storage within the HippoCamp ecosystem. The service will store embeddings with metadata to enable semantic search and context retrieval, allowing AI agents to maintain persistent memory across development sessions.

## Architecture Decisions
- **Database**: PostgreSQL with Entity Framework Core for embedding storage and vector operations
- **API Design**: RESTful HTTP API following HippoCamp conventions
- **Integration**: Aspire orchestration for development and monitoring
- **Data Model**: Simple entity structure with embeddings as float arrays and flexible metadata
- **Search Strategy**: Basic cosine similarity for embeddings, text search for content, metadata filtering

## Technical Approach
### Backend Services
- **Memory API Service**: Core REST API for CRUD operations and search
- **Data Models**: Memory entity with embeddings, metadata, and project isolation
- **Search Engine**: Embedding similarity search with metadata filtering
- **Database Layer**: Entity Framework Core with PostgreSQL vector extensions

### Infrastructure
- **Database**: PostgreSQL instance with vector extensions for embedding support
- **Deployment**: Aspire orchestration integration
- **Monitoring**: Built-in Aspire observability and logging
- **Testing**: xUnit v3 with comprehensive test coverage

## Implementation Strategy
- Start with core data model and basic CRUD operations
- Implement embedding similarity search using built-in vector operations
- Add project isolation and metadata filtering
- Focus on simplicity and extensibility over performance optimization
- Leverage existing HippoCamp patterns and infrastructure

## Task Breakdown Preview
High-level task categories that will be created:
- [ ] Database Setup: Configure PostgreSQL with EF Core and vector support
- [ ] Core Data Model: Implement Memory entity and DbContext
- [ ] API Infrastructure: Set up controllers, DTOs, and basic endpoints
- [ ] Memory CRUD Operations: Create, read, update, delete memory entries
- [ ] Search Implementation: Embedding similarity and metadata filtering
- [ ] Project Isolation: Implement project-based data separation
- [ ] Aspire Integration: Configure service registration and monitoring
- [ ] Testing Suite: Comprehensive unit and integration tests
- [ ] API Documentation: OpenAPI specs and usage documentation

## Dependencies
- **PostgreSQL Database**: Instance with vector extension support
- **Entity Framework Core**: Latest stable version for data access
- **HippoCamp Infrastructure**: Base project setup and conventions
- **Aspire Framework**: Integration with existing orchestration
- **xUnit v3**: Testing framework alignment with project standards

## Success Criteria (Technical)
- **Storage Capacity**: Handle 10GB+ of embedding data efficiently
- **Search Performance**: Sub-2-second response times for typical queries
- **API Reliability**: 99%+ uptime with comprehensive error handling
- **Data Integrity**: Zero data loss with proper referential integrity
- **Integration**: Seamless Aspire integration with full observability
- **Code Quality**: Meets HippoCamp standards with comprehensive test coverage

## Tasks Created
- [ ] #10 - Memory CRUD Operations (parallel: true)
- [ ] #11 - Embedding Similarity Search (parallel: true)
- [ ] #12 - Project Isolation and Security (parallel: true)
- [ ] #13 - Database Setup and Configuration (parallel: false)
- [ ] #14 - Core Data Models and Entities (parallel: false)
- [ ] #15 - API Infrastructure Setup (parallel: true)
- [ ] #7 - Aspire Integration and Monitoring (parallel: true)
- [ ] #8 - Comprehensive Testing Suite (parallel: false)
- [ ] #9 - API Documentation and Deployment (parallel: true)

Total tasks:        9
Parallel tasks:        6
Sequential tasks: 3
## Estimated Effort
- **Overall Timeline**: 6-8 weeks (following PRD phases)
- **Core Foundation**: 2 weeks (data model, basic CRUD, Aspire setup)
- **Search & Retrieval**: 2 weeks (embedding search, metadata filtering)
- **Integration & Testing**: 2 weeks (comprehensive tests, documentation)
- **Critical Path**: Database setup and vector extension configuration
