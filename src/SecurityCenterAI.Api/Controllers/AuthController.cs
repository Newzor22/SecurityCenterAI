using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurityCenterAI.Api.Contracts;
using SecurityCenterAI.Api.Services;
using SecurityCenterAI.Domain.Entities;
using SecurityCenterAI.Infrastructure.Persistence;

namespace SecurityCenterAI.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    AppDbContext dbContext,
    IPasswordHasher<User> passwordHasher,
    ITokenService tokenService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        if (await dbContext.Users.AnyAsync(
                user => user.Email == normalizedEmail,
                cancellationToken))
        {
            return Conflict(new ProblemDetails
            {
                Title = "El correo ya está registrado.",
                Status = StatusCodes.Status409Conflict
            });
        }

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = normalizedEmail,
            PasswordHash = string.Empty
        };
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = CreateResponse(user);
        return CreatedAtAction(nameof(Register), response);
    }

    [HttpPost("login")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await dbContext.Users.SingleOrDefaultAsync(
            item => item.Email == normalizedEmail,
            cancellationToken);

        if (user is null ||
            passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Credenciales inválidas.",
                Status = StatusCodes.Status401Unauthorized
            });
        }

        return Ok(CreateResponse(user));
    }

    private AuthResponse CreateResponse(User user)
    {
        var (token, expiresAt) = tokenService.Create(user);
        return new AuthResponse(
            token,
            expiresAt,
            new UserResponse(user.Id, user.Name, user.Email));
    }
}
