namespace ResultPattern.API.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Age { get; set; }
}

public record CreateUserRequest(string Name, string Email, int Age);
public record UpdateUserRequest(string Name, int Age);
public record ErrorResponse(string Message);
