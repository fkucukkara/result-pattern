---
agent: 'agent'
tools: ['changes', 'codebase', 'editFiles', 'problems']
description: 'Review Result Pattern usage across the codebase and suggest improvements'
---

# Result Pattern Review

Analyze this ASP.NET Core Minimal API project for correct and idiomatic usage of the Result Pattern. Review every `.cs` file and report findings.

## Checklist

### Result<T> Usage
- All service/repository methods return `Result<T>` instead of throwing for expected failures
- `Result<T>.Success(value)` is used for happy paths
- `Result<T>.Failure("message")` is used for validation errors, not-found, and duplicate cases
- No code accesses `.Value` without first checking `IsSuccess`
- Implicit conversion (`return entity;`) is used where it simplifies code

### Error Message Consistency
- Messages follow the pattern: `"Entity with [Field] {value} was not found"`
- Messages are user-safe — no stack traces or internal details leaked
- Messages are consistent between service and repository layers

### Endpoint Mapping
- Endpoints use `.ToHttpResponse()` or `.ToHttpResponseWithNotFound()` extension methods
- Return types use `Results<Ok<T>, BadRequest<ErrorResponse>>` or the NotFound variant
- No manual `if/else` mapping that duplicates what the extensions provide

### Async Patterns
- All async methods are suffixed with `Async`
- `CancellationToken` is accepted and forwarded where applicable
- No `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()` blocking calls
- `Task.WhenAll` is used for independent concurrent operations

### Architecture
- DTOs are records; entities are classes
- Validation lives in the service layer, not in endpoints or repositories
- Repository handles data access only; business logic stays in services
- DI registrations match lifetimes (Singleton for in-memory repo, Scoped for services)

## Output

Report in three sections:
### ✅ Correct Patterns Found
### ⚠️ Issues & Risks
### 🔧 Suggested Improvements (with code examples)
