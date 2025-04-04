using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.API.Data;
using AuthService.API.DTO;
using AuthService.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReservationSystem.Shared;
using ReservationSystem.Shared.Clients;
using ReservationSystem.Shared.Results;

namespace AuthService.API.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly NetworkHttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly AuthDbContext _dbContext;

    public AuthController(UserManager<ApplicationUser> userManager, NetworkHttpClient httpClient,
        IConfiguration configuration, AuthDbContext dbContext)
    {
        _userManager = userManager;
        _httpClient = httpClient;
        _configuration = configuration;
        _dbContext = dbContext;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var existingEmail = await _userManager.FindByEmailAsync(model.Email);
        if (existingEmail != null)
            return BadRequest(new ApiResult<object>(null, false, "User with this email already exists."));

        var existingUsername = await _userManager.FindByNameAsync(model.UserName);
        if (existingUsername != null)
            return BadRequest(new ApiResult<object>(null, false, "User with this username already exists."));

        var url = ServiceEndpoints.UserService.CreateUser;
        var newUser = new
        {
            Username = model.UserName,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email.ToLower(),
        };

        var response = await _httpClient.PostAsync<object, object>(url, newUser, CancellationToken.None);

        if (!response.Success)
            return BadRequest(new ApiResult<object>(null, false, response.Message));

        var user = new ApplicationUser
        {
            Email = model.Email,
            UserName = model.UserName,
            UserId = int.Parse(response.Data.ToString())
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
            return BadRequest(new ApiResult<object>(result.Errors, false, result.Errors.ToString()));

        return Ok(new ApiResult<object>(new { Id = user.UserId }));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResult<object>(null, false, "Invalid data."));

        ApplicationUser user = null;

        if (!string.IsNullOrEmpty(model.UsernameOrEmail))
        {
            // No manual normalization here
            user = await _userManager.FindByEmailAsync(model.UsernameOrEmail)
                   ?? await _userManager.FindByNameAsync(model.UsernameOrEmail);
        }

        if (user == null)
            return BadRequest(new ApiResult<object>(null, false, "Invalid username/email or password."));

        var result = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!result)
            return BadRequest(new ApiResult<object>(null, false, "Invalid username/email or password."));

        var token = GenerateJwtToken(user);
        return Ok(new ApiResult<string>(token));
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var secretKey = _configuration["JWT_SECRET_KEY"];
        var issuer = _configuration["JWT_ISSUER"];
        var audience = _configuration["JWT_AUDIENCE"];

        var claims = new List<Claim>
        {
            new Claim("userid", user.Id),
            new Claim("username", user.UserName!)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [Authorize]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userid");

        if (userIdClaim == null)
            return Unauthorized(new ApiResult<object>(null, false, "Token does not contain user ID."));

        var userId = userIdClaim.Value;

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return BadRequest(new ApiResult<object>(null, false, "User not found."));

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

        if (!result.Succeeded)
            return BadRequest(new ApiResult<object>(result.Errors, false, "Password reset failed."));

        return Ok(new ApiResult<object>(
            new { UserId = user.Id, Email = user.Email },
            true,
            "Password reset successfully."));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();

            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userid");
            var usernameClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "username");


            if (userIdClaim == null || usernameClaim == null)
            {
                return Unauthorized(new
                {
                    message = "Token is invalid or claims are missing",
                    claims = HttpContext.User.Claims.Select(c => new { c.Type, c.Value })
                });
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userIdClaim.Value);

            if (user == null)
            {
                return Unauthorized(new { message = "User not found in database." });
            }

            var expClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "exp");

            if (expClaim == null || !long.TryParse(expClaim.Value, out var expUnix))
            {
                return Unauthorized(new { message = "Missing or invalid exp claim" });
            }

            var expirationDateUtc = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;

            var tokenBlackList = new TokenBlackList
            {
                UserId = user.Id,
                Token = token,
                ExpirationDate = expirationDateUtc
            };

            _dbContext.TokenBlackLists.Add(tokenBlackList);
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                message = "User logged out successfully.",
                userid = user.Id,
                username = user.UserName,
                claims = HttpContext.User.Claims.Select(c => new { c.Type, c.Value })
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($" Exception in Logout: {ex.Message}");
            return StatusCode(500, new { message = "An error occurred.", exception = ex.Message });
        }
    }
}