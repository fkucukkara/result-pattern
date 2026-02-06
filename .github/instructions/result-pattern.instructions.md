---
description: 'Result Pattern conventions for this ASP.NET Core Minimal API project'
applyTo: '**/*.cs'
---

# Result Pattern Instructions

## Core Pattern

This project uses the **Result Pattern** to make failures explicit in method signatures instead of throwing exceptions for expected error cases.

### Result<T> Type
- Use `Result<T>.Success(value)` for successful outcomes
- Use `Result<T>.Failure("message")` for expected failures (validation, not-found, duplicates)
- Throw exceptions only for truly unexpected/exceptional conditions (infrastructure failures)
- Leverage the implicit conversion: `return user;` auto-wraps in `Result<T>.Success(user)`

### Service Layer
- All service methods return `Result<T>` or `Result<object?>` for void-like operations
- Validate inputs at the top of each method; return `Failure` immediately on invalid input
- Chain results: check `IsFailure` on dependent calls before proceeding
- Never catch exceptions to convert them to `Result` — let infrastructure errors bubble

### Extension Methods
- Map results to HTTP responses using `ToHttpResponse()` or `ToHttpResponseWithNotFound()`
- Use `Results<Ok<T>, BadRequest<ErrorResponse>>` for typed endpoint signatures
- The "not found" variant inspects the error message for `"not found"` (case-insensitive)

## Naming & Style
- Follow PascalCase for public members, camelCase for privates
- Suffix async methods with `Async`; always accept `CancellationToken` where applicable
- Use `is null` / `is not null` — never `== null`
- Prefer file-scoped namespaces and records for DTOs

## Error Messages
- Use descriptive, user-facing messages: `"User with ID {id} was not found"`
- Keep messages consistent across repository and service layers
- Never expose stack traces or internal details in error messages

## Adding New Endpoints
1. Define the model/DTO as a `record` in `Models/`
2. Add repository method returning `Result<T>` in the repository interface and implementation
3. Add service method with validation logic returning `Result<T>`
4. Map the endpoint in `Program.cs` using `MapGet`/`MapPost`/etc. with `.ToHttpResponse()` or `.ToHttpResponseWithNotFound()`

## Anti-Patterns to Avoid
- Throwing exceptions for validation failures or not-found cases
- Using `.Value` without checking `IsSuccess` first
- Returning `null` instead of `Result<T>.Failure(...)`
- Mixing Result pattern and exception-based error handling in the same flow
