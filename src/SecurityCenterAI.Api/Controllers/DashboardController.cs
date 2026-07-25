using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurityCenterAI.Infrastructure.Persistence;

namespace SecurityCenterAI.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/dashboard")]
public sealed class DashboardController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value, out var userId))
        {
            return Unauthorized();
        }

        var analyses = await dbContext.SecurityAnalyses
            .Where(item => item.UserId == userId)
            .OrderByDescending(item => item.AnalyzedAtUtc)
            .Select(item => new { item.Id, item.Score, item.AnalyzedAtUtc })
            .Take(5)
            .ToListAsync(cancellationToken);

        return Ok(new
        {
            SecurityScore = analyses.FirstOrDefault()?.Score,
            TotalAnalyses = await dbContext.SecurityAnalyses.CountAsync(
                item => item.UserId == userId,
                cancellationToken),
            RecentAnalyses = analyses
        });
    }
}
