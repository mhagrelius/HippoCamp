---
task: 10
title: Memory CRUD Operations
analyzed: 2025-09-05T02:15:00Z
parallel_streams: 4
---

# Analysis for Task #10: Memory CRUD Operations

## Overview
Task #10 focuses on implementing comprehensive CRUD operations for the Memory Store API. With the core data models from Task #14 now complete (Memory entity, DTOs, MemoryDbContext), this task can be parallelized across four distinct work streams that handle different aspects of the CRUD functionality while minimizing code conflicts.

The task requires:
- Core CRUD operations (Create, Read, Update, Delete)  
- Validation and error handling
- Batch operations with transaction support
- API endpoints with proper HTTP status codes
- Soft delete implementation

## Parallel Work Streams

### Stream A: Core Memory Service Layer
**Focus**: Business logic layer for memory operations
**Agent Type**: general-purpose
**Files**: 
- `src/HippoCamp.MemoryStore/Services/IMemoryService.cs`
- `src/HippoCamp.MemoryStore/Services/MemoryService.cs`
- `src/HippoCamp.MemoryStore/Models/ApiErrorResponse.cs`

**Work Description**:
Implement the core service layer that handles all memory operations including:
- Single memory CRUD operations (Create, GetById, Update, SoftDelete)
- Project-based memory retrieval with filtering
- Embedding validation and conversion (float[] to Vector)
- Business rule validation (content length, project ID validation)
- Soft delete logic with IsDeleted flag management
- Error handling and result wrapping

**Dependencies**: None (uses existing DTOs and entities)
**Effort**: Large (L)
**Priority**: High

### Stream B: Batch Operations Service
**Focus**: Batch and bulk operations with transaction support
**Agent Type**: general-purpose
**Files**:
- `src/HippoCamp.MemoryStore/Services/IBatchMemoryService.cs`
- `src/HippoCamp.MemoryStore/Services/BatchMemoryService.cs`
- `src/HippoCamp.MemoryStore/DTOs/BatchCreateMemoryDto.cs`
- `src/HippoCamp.MemoryStore/DTOs/BatchOperationResultDto.cs`

**Work Description**:
Implement batch operations service that handles:
- Batch create operations (max 100 memories per batch)
- Batch update operations with transaction rollback
- Bulk deprecation by criteria (project, type, date range)
- Transaction management and rollback on validation failure
- Progress reporting for large operations
- Batch validation with detailed error reporting

**Dependencies**: None (can work independently using existing models)
**Effort**: Medium (M)
**Priority**: Medium

### Stream C: API Controllers and Endpoints
**Focus**: REST API layer with HTTP handling
**Agent Type**: general-purpose
**Files**:
- `src/HippoCamp.MemoryStore/Controllers/MemoriesController.cs`
- `src/HippoCamp.MemoryStore/Controllers/BatchController.cs`

**Work Description**:
Implement API controllers providing all required endpoints:
- `POST /api/memories` - Create single memory
- `GET /api/memories/{id}` - Get memory by ID  
- `PUT /api/memories/{id}` - Update existing memory
- `DELETE /api/memories/{id}` - Soft delete memory
- `GET /api/projects/{projectId}/memories` - Get project memories
- `POST /api/memories/batch` - Batch create operations
- Proper HTTP status codes and error responses
- Request/response validation and model binding
- Swagger documentation attributes

**Dependencies**: Streams A and B (services must be implemented first)
**Effort**: Medium (M)
**Priority**: Medium

### Stream D: Validation and Error Handling
**Focus**: Input validation, error responses, and middleware
**Agent Type**: general-purpose
**Files**:
- `src/HippoCamp.MemoryStore/Validation/MemoryValidator.cs`
- `src/HippoCamp.MemoryStore/Validation/EmbeddingValidator.cs`
- `src/HippoCamp.MemoryStore/Middleware/GlobalExceptionMiddleware.cs`
- `src/HippoCamp.MemoryStore/Extensions/ServiceCollectionExtensions.cs`

**Work Description**:
Implement comprehensive validation and error handling:
- Memory content validation (required, max 10KB)
- Embedding validation (dimensions, format consistency) 
- ProjectId validation (required, format validation)
- Metadata schema validation for JSON structure
- Global exception handling middleware
- Standardized error response formatting
- Validation error aggregation and reporting
- Service registration extensions for DI

**Dependencies**: Can work independently using existing DTOs
**Effort**: Medium (M)  
**Priority**: High

## Coordination Notes

### Service Registration
All services must be registered in `Program.cs` or through extension methods. Stream D should provide the `ServiceCollectionExtensions.cs` to centralize registration.

### Error Response Consistency
All streams must use the standardized `ApiErrorResponse` model created in Stream A for consistent error formatting across the API.

### Transaction Scope
Stream B (Batch Operations) must coordinate with Stream A to ensure single operations can participate in batch transactions when needed.

### Testing Approach
Each stream should include comprehensive unit tests. Integration tests should be added after all streams are complete to test end-to-end functionality.

## Critical Path

**Blocking Relationships:**
1. Stream A must complete basic service interfaces before Stream C can implement controllers
2. Stream B can work independently but should align with Stream A error handling patterns  
3. Stream D can work independently but should coordinate error response models with Stream A
4. Stream C depends on both Stream A and Stream B completion

**Optimal Execution Order:**
1. **Phase 1 (Parallel)**: Stream A + Stream D + Stream B (independent work)
2. **Phase 2**: Stream C (depends on A and B completion)
3. **Phase 3**: Integration testing and coordination fixes

**Estimated Timeline:**
- Phase 1: 2-3 days parallel development
- Phase 2: 1-2 days controller implementation  
- Phase 3: 1 day integration and testing
- **Total**: 4-6 days with proper coordination