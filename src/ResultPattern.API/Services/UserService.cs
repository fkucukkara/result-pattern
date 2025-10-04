using ResultPattern.API.Common;
using ResultPattern.API.Models;

namespace ResultPattern.API.Services;

public interface IUserService
{
    Task<Result<User>> GetUserByIdAsync(int id);
    Task<Result<User>> CreateUserAsync(CreateUserRequest request);
    Task<Result<User>> UpdateUserAsync(int id, UpdateUserRequest request);
    Task<Result<object?>> DeleteUserAsync(int id);
}

public class UserService(IUserRepository userRepository) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<Result<User>> GetUserByIdAsync(int id)
    {
        if (id <= 0)
            return Result<User>.Failure("User ID must be greater than 0");

        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<Result<User>> CreateUserAsync(CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<User>.Failure("Name is required");

        if (string.IsNullOrWhiteSpace(request.Email))
            return Result<User>.Failure("Email is required");

        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser.IsSuccess)
            return Result<User>.Failure("Email already exists");

        var user = new User
        {
            Id = Random.Shared.Next(1, 1000),
            Name = request.Name,
            Email = request.Email,
            Age = request.Age
        };

        return await _userRepository.AddAsync(user);
    }

    public async Task<Result<User>> UpdateUserAsync(int id, UpdateUserRequest request)
    {
        var getUserResult = await GetUserByIdAsync(id);
        if (getUserResult.IsFailure)
            return getUserResult;

        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<User>.Failure("Name is required");

        var user = getUserResult.Value;
        user.Name = request.Name;
        user.Age = request.Age;

        return await _userRepository.UpdateAsync(user);
    }

    public async Task<Result<object?>> DeleteUserAsync(int id)
    {
        var getUserResult = await GetUserByIdAsync(id);
        if (getUserResult.IsFailure)
            return Result<object?>.Failure(getUserResult.Error);

        return await _userRepository.DeleteAsync(id);
    }
}
