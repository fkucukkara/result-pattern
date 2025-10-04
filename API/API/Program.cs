using API.Extensions;
using API.Models;
using API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var users = app.MapGroup("/api/users");

users.MapGet("/{id:int}", async (int id, IUserService userService) =>
{
    var result = await userService.GetUserByIdAsync(id);
    return result.ToHttpResponseWithNotFound();
});

users.MapPost("/", async (CreateUserRequest request, IUserService userService) =>
{
    var result = await userService.CreateUserAsync(request);
    return result.ToHttpResponse();
});

users.MapPut("/{id:int}", async (int id, UpdateUserRequest request, IUserService userService) =>
{
    var result = await userService.UpdateUserAsync(id, request);
    return result.ToHttpResponseWithNotFound();
});

users.MapDelete("/{id:int}", async (int id, IUserService userService) =>
{
    var result = await userService.DeleteUserAsync(id);
    return result.ToHttpResponseWithNotFound();
});

app.Run();
