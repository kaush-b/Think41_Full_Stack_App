using System.Security.Claims;
using HRBMS.Core.Options;
using HRBMS.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HRBMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly JwtOptions _jwtOptions;

    public AuthController(IUserService userService, IJwtTokenService jwtTokenService, IOptions<JwtOptions> jwtOptions)
    {
        _userService = userService;
        _jwtTokenService = jwtTokenService;
        _jwtOptions = jwtOptions.Value;
    }

    public record RegisterGuestRequest(string Username, string Email, string Password);
    public record RegisterStaffRequest(string Username, string Email, string Password, IEnumerable<string> Roles);
    public record LoginRequest(string Username, string Password);

    [HttpPost("register/guest")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterGuest([FromBody] RegisterGuestRequest request, CancellationToken cancellationToken)
    {
        var user = await _userService.RegisterGuestAsync(request.Username, request.Email, request.Password, cancellationToken);
        return Ok(new { user.Id, user.Username, user.Email });
    }

    [HttpPost("register/staff")]
    [Authorize(Policy = "RequireAdmin")]
    public async Task<IActionResult> RegisterStaff([FromBody] RegisterStaffRequest request, CancellationToken cancellationToken)
    {
        var user = await _userService.RegisterStaffAsync(request.Username, request.Email, request.Password, request.Roles, cancellationToken);
        return Ok(new { user.Id, user.Username, user.Email, Roles = request.Roles });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _userService.ValidateCredentialsAsync(request.Username, request.Password, cancellationToken);
        if (user == null) return Unauthorized();
        var roles = await _userService.GetRoleNamesAsync(user.Id, cancellationToken);
        var token = _jwtTokenService.GenerateToken(user, roles);
        return Ok(new { token, expiresInMinutes = _jwtOptions.ExpiryMinutes });
    }
}