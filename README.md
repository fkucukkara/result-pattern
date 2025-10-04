# Result Pattern Demo 

A comprehensive demonstration of the **Result Pattern** in .NET 9, showing how to handle errors gracefully without throwing exceptions in a real-world ASP.NET Core Minimal API.

## What is the Result Pattern?

The Result pattern is a functional programming approach that makes failures explicit in method signatures. Instead of throwing exceptions, you return a Result object that represents either success or failure, making error handling predictable and performant.

## Key Benefits

- **Performance**: No exception overhead for expected failures
- **Explicit Error Handling**: Errors are part of method signatures, making them impossible to ignore
- **Predictable**: Clear success/failure paths with no hidden control flow
- **Clean Code**: No scattered try-catch blocks throughout your application
- **Composable**: Results can be chained and transformed easily
- **Type Safety**: Compile-time guarantees about error handling

## Core Implementation

### Result Types

```csharp
// Non-generic Result for operations without return values
Result operationResult = Result.Success();
Result failureResult = Result.Failure("Something went wrong");

// Generic Result<T> for operations that return values
Result<User> userResult = Result<User>.Success(user);
Result<User> notFoundResult = Result<User>.Failure("User not found");
```

### Basic Usage

```csharp
public async Task<Result<User>> GetUserAsync(int id)
{
    if (id <= 0)
        return Result<User>.Failure("Invalid user ID");
    
    var user = await _repository.GetByIdAsync(id);
    return user is not null 
        ? Result<User>.Success(user)
        : Result<User>.Failure("User not found");
}

// Using the result
var result = await GetUserAsync(123);
if (result.IsSuccess)
{
    Console.WriteLine($"Found user: {result.Value.Name}");
}
else
{
    Console.WriteLine($"Error: {result.Error}");
}
```

### HTTP Response Extensions

The demo includes custom extensions to convert Results to appropriate HTTP responses:

```csharp
// Converts Result<T> to HTTP responses with proper status codes
return result.ToHttpResponse(); // 200 OK or 400 Bad Request

// For operations that can return 404 Not Found
return result.ToHttpResponseWithNotFound(); // 200 OK, 400 Bad Request, or 404 Not Found
```

## API Endpoints

The demo includes a complete User management API demonstrating the Result pattern in action:

### User Operations
- **GET** `/api/users/{id}` - Get user by ID
  - Returns 200 with user data on success
  - Returns 404 when user not found
  - Returns 400 for invalid ID (≤ 0)

- **POST** `/api/users` - Create new user
  - Returns 200 with created user on success
  - Returns 400 for validation errors (duplicate email, invalid age)

- **PUT** `/api/users/{id}` - Update user
  - Returns 200 with updated user on success
  - Returns 404 when user not found
  - Returns 400 for validation errors

- **DELETE** `/api/users/{id}` - Delete user
  - Returns 200 on successful deletion
  - Returns 404 when user not found

### Request/Response Models

**Create User Request:**
```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "age": 30
}
```

**Update User Request:**
```json
{
  "name": "John Updated",
  "age": 31
}
```

**User Response:**
```json
{
  "id": 123,
  "name": "John Doe",
  "email": "john@example.com",
  "age": 30
}
```

**Error Response:**
```json
{
  "message": "Email already exists",
  "code": null
}
```

## Running the Demo

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd result-pattern-101
   ```

2. **Navigate to the API folder**
   ```bash
   cd API/API
   ```

3. **Build and run the application**
   ```bash
   dotnet build
   dotnet run
   ```

4. **Access the API**
   - API will be available at: `http://localhost:5065`
   - Swagger documentation: `http://localhost:5065/openapi/v1.json`
   - Use the provided `API.http` file for testing requests

## Testing the API

The project includes an `API.http` file with comprehensive test cases covering:

- ✅ **Success scenarios**: Valid operations that work as expected
- ❌ **Failure scenarios**: Invalid inputs, not found cases, and validation errors
- 🔍 **Edge cases**: Boundary conditions and error handling

### Quick Test Sequence

1. **Create a user**: `POST /api/users` with valid data
2. **Get the user**: `GET /api/users/1` to retrieve the created user
3. **Update the user**: `PUT /api/users/1` with new data
4. **Try invalid operations**: Test error cases like invalid IDs or duplicate emails
5. **Delete the user**: `DELETE /api/users/1`

## Example Flows

### Creating a User (Success)

**Request:**
```bash
POST /api/users
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john@example.com",
  "age": 30
}
```

**Response (200 OK):**
```json
{
  "id": 1,
  "name": "John Doe",
  "email": "john@example.com",
  "age": 30
}
```

### Creating a User (Validation Error)

**Request:**
```bash
POST /api/users
Content-Type: application/json

{
  "name": "Jane Smith",
  "email": "john@example.com",  // Duplicate email
  "age": 25
}
```

**Response (400 Bad Request):**
```json
{
  "message": "Email already exists"
}
```

### Getting a User (Not Found)

**Request:**
```bash
GET /api/users/999
```

**Response (404 Not Found):**
```json
{
  "message": "User not found"
}
```

## Project Structure

```
API/
├── API.sln                          # Solution file
└── API/
    ├── API.csproj                   # Project file (.NET 9)
    ├── API.http                     # HTTP test requests
    ├── Program.cs                   # Application entry point & Minimal API setup
    ├── appsettings.json             # Configuration
    ├── appsettings.Development.json # Development configuration
    ├── Common/
    │   └── Result.cs                # Core Result pattern implementation
    ├── Extensions/
    │   └── ResultExtensions.cs      # HTTP response extensions
    ├── Models/
    │   └── ApiModels.cs             # Request/Response models
    ├── Services/
    │   ├── UserRepository.cs        # Data access using Result pattern
    │   └── UserService.cs           # Business logic using Result pattern
    └── Properties/
        └── launchSettings.json      # Launch configuration
```

## Key Implementation Files

### 🎯 **Common/Result.cs**
Core Result pattern implementation featuring:
- Generic and non-generic Result types
- Implicit conversion operators
- Fluent API for creating success/failure results

### 🔌 **Extensions/ResultExtensions.cs**
HTTP response extensions that convert Results to appropriate HTTP status codes:
- `ToHttpResponse()` - 200 OK or 400 Bad Request
- `ToHttpResponseWithNotFound()` - 200 OK, 400 Bad Request, or 404 Not Found

### 🏢 **Services/UserService.cs**
Business logic layer demonstrating:
- Input validation using the Result pattern
- Async operations with Result return types
- Composition of multiple Result-based operations

### 💾 **Services/UserRepository.cs**
Data access layer showing:
- In-memory storage with Result-based operations
- Async database simulation
- Common data access patterns with Results

### 🌐 **Program.cs**
ASP.NET Core Minimal API setup featuring:
- Dependency injection configuration
- Route mapping with Result-to-HTTP conversion
- Clean separation of concerns

## Result Pattern Benefits Demonstrated

### 1. **Performance** 
No exception overhead for expected failures like "user not found" or validation errors.

### 2. **Explicit Error Handling**
Method signatures clearly indicate what can go wrong:
```csharp
Task<Result<User>> GetUserByIdAsync(int id)  // Can fail
Task<Result> DeleteUserAsync(int id)         // Can fail
```

### 3. **Predictable Control Flow**
No hidden exceptions - all error paths are explicit and handled at the call site.

### 4. **Clean API Design**
HTTP endpoints automatically return appropriate status codes based on Result state.

### 5. **Composability**
Results can be chained and transformed easily without nested try-catch blocks.

## Advanced Patterns

The demo showcases several advanced Result pattern techniques:

- **Repository Pattern Integration**: Data access layer returns Results
- **Service Layer Composition**: Business logic combines multiple Result operations
- **HTTP Response Mapping**: Automatic conversion from Results to HTTP responses
- **Validation Integration**: Input validation using the Result pattern
- **Async/Await Support**: Full async support throughout the pipeline

## Best Practices Demonstrated

✅ **Use Results for expected failures** (validation, not found, business rule violations)  
✅ **Keep exception handling for unexpected failures** (network issues, database connectivity)  
✅ **Make error messages user-friendly and actionable**  
✅ **Use appropriate HTTP status codes based on Result state**  
✅ **Leverage implicit conversions for clean code**  
✅ **Compose Results using extension methods**  

The Result pattern provides a clean, performant, and predictable way to handle errors in modern .NET applications, making your code more robust and maintainable.
