---
name: result-pattern
description: 'Scaffold and extend Result Pattern types in .NET projects. Use this skill when adding new Result variants, extending Result<T> with new mapping methods, or integrating the Result Pattern into new service/repository layers.'
---

# Result Pattern Skill

## Overview

This skill helps scaffold and extend the Result Pattern in ASP.NET Core Minimal API projects. It covers creating `Result<T>` types, HTTP response mapping extensions, and wiring services that return results instead of throwing exceptions.

## Prerequisites

- .NET 10 SDK (or later) installed
- `dotnet` CLI available on your `PATH`
- An ASP.NET Core project using Minimal APIs

## Core Concepts

### Result<T> Anatomy

A well-formed `Result<T>` type must provide:

| Member | Purpose |
|---|---|
| `IsSuccess` / `IsFailure` | Status check |
| `Value` | Accessible only on success; throws on failure |
| `Error` | Accessible only on failure; throws on success |
| `Success(T value)` | Factory for happy path |
| `Failure(string error)` | Factory for expected failure |
| `implicit operator` | Auto-wrap `T` → `Result<T>.Success(T)` |

### When to Return Failure vs Throw

| Scenario | Approach |
|---|---|
| Validation error | `Result<T>.Failure(...)` |
| Entity not found | `Result<T>.Failure(...)` |
| Duplicate / conflict | `Result<T>.Failure(...)` |
| Database connection lost | **Throw** exception |
| Null argument to public API | **Throw** `ArgumentNullException` |

## Workflows

### Adding a New Entity with Result Pattern

1. **Define the model** as a `record` in `Models/`:
   ```csharp
   public record CreateProductRequest(string Name, decimal Price);
   public record Product { public int Id { get; set; } public string Name { get; set; } }
   ```

2. **Add repository interface & implementation** returning `Result<T>`:
   ```csharp
   public interface IProductRepository
   {
       Task<Result<Product>> GetByIdAsync(int id);
       Task<Result<Product>> AddAsync(Product product);
   }
   ```

3. **Add service with validation** returning `Result<T>`:
   ```csharp
   public class ProductService(IProductRepository repo) : IProductService
   {
       public async Task<Result<Product>> CreateAsync(CreateProductRequest request)
       {
           if (string.IsNullOrWhiteSpace(request.Name))
               return Result<Product>.Failure("Name is required");

           var product = new Product { Id = Random.Shared.Next(1, 1000), Name = request.Name };
           return await repo.AddAsync(product);
       }
   }
   ```

4. **Map the endpoint** in `Program.cs`:
   ```csharp
   products.MapPost("/", async (CreateProductRequest req, IProductService svc) =>
   {
       var result = await svc.CreateAsync(req);
       return result.ToHttpResponse();
   });
   ```

### Adding a New Response Extension

When you need a new HTTP status mapping (e.g., `Conflict`), add a method to `ResultExtensions`:

```csharp
public static Results<Ok<T>, Conflict<ErrorResponse>, BadRequest<ErrorResponse>> ToHttpResponseWithConflict<T>(
    this Result<T> result)
{
    if (result.IsSuccess)
        return TypedResults.Ok(result.Value);

    var isConflict = result.Error.Contains("already exists", StringComparison.OrdinalIgnoreCase);

    return isConflict
        ? TypedResults.Conflict(new ErrorResponse(result.Error))
        : TypedResults.BadRequest(new ErrorResponse(result.Error));
}
```

## Verification

After scaffolding, always:
1. Run `dotnet build` — ensure zero errors/warnings
2. Test the happy path with a valid request
3. Test the failure path with invalid input
4. Test the not-found path (if applicable) with a missing entity
