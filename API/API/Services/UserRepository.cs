using API.Common;
using API.Models;
using System.Collections.Concurrent;

namespace API.Services;

public interface IUserRepository
{
    Task<Result<User>> GetByIdAsync(int id);
    Task<Result<User>> GetByEmailAsync(string email);
    Task<Result<User>> AddAsync(User user);
    Task<Result<User>> UpdateAsync(User user);
    Task<Result<object?>> DeleteAsync(int id);
}

public class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<int, User> _users = new();

    public InMemoryUserRepository()
    {
        // Seed with initial data
        _users.TryAdd(1, new User { Id = 1, Name = "John Doe", Email = "john@example.com", Age = 30 });
        _users.TryAdd(2, new User { Id = 2, Name = "Jane Smith", Email = "jane@example.com", Age = 25 });
    }

    public async Task<Result<User>> GetByIdAsync(int id)
    {
        await Task.Delay(10); // Simulate async operation

        return _users.TryGetValue(id, out var user)
            ? Result<User>.Success(user)
            : Result<User>.Failure($"User with ID {id} was not found");
    }

    public async Task<Result<User>> GetByEmailAsync(string email)
    {
        await Task.Delay(10);

        var user = _users.Values.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        
        return user is not null
            ? Result<User>.Success(user)
            : Result<User>.Failure($"User with email {email} was not found");
    }

    public async Task<Result<User>> AddAsync(User user)
    {
        await Task.Delay(10);

        if (_users.TryAdd(user.Id, user))
            return Result<User>.Success(user);

        return Result<User>.Failure($"Failed to add user with ID {user.Id}");
    }

    public async Task<Result<User>> UpdateAsync(User user)
    {
        await Task.Delay(10);

        if (_users.ContainsKey(user.Id))
        {
            _users[user.Id] = user;
            return Result<User>.Success(user);
        }

        return Result<User>.Failure($"User with ID {user.Id} was not found");
    }

    public async Task<Result<object?>> DeleteAsync(int id)
    {
        await Task.Delay(10);

        return _users.TryRemove(id, out _)
            ? Result<object?>.Success(null)
            : Result<object?>.Failure($"User with ID {id} was not found");
    }
}
