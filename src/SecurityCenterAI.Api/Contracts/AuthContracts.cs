using System.ComponentModel.DataAnnotations;

namespace SecurityCenterAI.Api.Contracts;

public sealed record RegisterRequest(
    [property: Required, StringLength(100, MinimumLength = 2)] string Name,
    [property: Required, EmailAddress] string Email,
    [property: Required, MinLength(8)] string Password);

public sealed record LoginRequest(
    [property: Required, EmailAddress] string Email,
    [property: Required] string Password);

public sealed record AuthResponse(
    string AccessToken,
    DateTime ExpiresAtUtc,
    UserResponse User);

public sealed record UserResponse(Guid Id, string Name, string Email);
