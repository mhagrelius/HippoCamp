namespace HippoCamp.MemoryStore.Models;

/// <summary>
/// Defines the types of memories that can be stored in the memory store.
/// </summary>
public enum MemoryType
{
    /// <summary>
    /// Records architectural decisions made during development.
    /// </summary>
    ArchitecturalDecision,
    
    /// <summary>
    /// Captures reusable code patterns and best practices.
    /// </summary>
    CodePattern,
    
    /// <summary>
    /// Documents bug patterns and their solutions.
    /// </summary>
    BugPattern,
    
    /// <summary>
    /// Stores performance optimization techniques and results.
    /// </summary>
    PerformanceOptimization,
    
    /// <summary>
    /// Records user preferences and customizations.
    /// </summary>
    UserPreference
}