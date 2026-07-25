using Microsoft.AspNetCore.Identity;
using SecurityCenterAI.Domain.Entities;

namespace SecurityCenterAI.Tests;

public sealed class AuthenticationTests
{
    [Fact]
    public void PasswordHasher_DoesNotStorePlainText_AndValidatesPassword()
    {
        var user = new User
        {
            Name = "Test User",
            Email = "test@example.com",
            PasswordHash = string.Empty
        };
        var hasher = new PasswordHasher<User>();

        user.PasswordHash = hasher.HashPassword(user, "SecurePass123!");

        Assert.NotEqual("SecurePass123!", user.PasswordHash);
        Assert.NotEqual(
            PasswordVerificationResult.Failed,
            hasher.VerifyHashedPassword(user, user.PasswordHash, "SecurePass123!"));
    }
}
