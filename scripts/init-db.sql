-- Initialize HippoCamp MemoryStore Database
-- This script runs on container startup

-- Create the vector extension if it doesn't exist
CREATE EXTENSION IF NOT EXISTS vector;

-- Verify the extension is available
SELECT * FROM pg_extension WHERE extname = 'vector';

-- Create a test table to verify pgvector functionality
CREATE TABLE IF NOT EXISTS vector_test (
    id SERIAL PRIMARY KEY,
    embedding vector(1536),  -- Standard OpenAI embedding dimension
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Insert a test vector to verify functionality
INSERT INTO vector_test (embedding) 
VALUES (ARRAY(SELECT random() FROM generate_series(1, 1536))::vector)
ON CONFLICT DO NOTHING;

-- Clean up test table (comment out if you want to keep for debugging)
-- DROP TABLE IF EXISTS vector_test;