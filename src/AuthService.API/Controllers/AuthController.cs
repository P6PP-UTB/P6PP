using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservationSystem.Shared.Results;
using AuthService.API.DTO;
using AuthService.API.Interfaces;

namespace AuthService.API.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : Controller
{
    private readonly IUserAuthService _authService;

    public AuthController(IUserAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var result = await _authService.RegisterAsync(model);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResult<object>(null, false, "Invalid data."));

        var result = await _authService.LoginAsync(model);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
    {
        if (!ModelState.IsValid)
        {
            foreach (var kvp in ModelState)
            {
                foreach (var error in kvp.Value.Errors)
                {
                    Console.WriteLine($"Validation error - Field: {kvp.Key}, Error: {error.ErrorMessage}");
                }
            }
            return BadRequest(ModelState);
        }

        var result = await _authService.ChangePasswordAsync(HttpContext, model);
        return result.Success ? Ok(result) : BadRequest(result);
    }


    // This endpoint is for anonymous users to request a password reset.
    [AllowAnonymous]
    [HttpGet("reset-password")]
    public async Task<IActionResult> RequestPasswordReset([FromQuery] string email)
    {
        var result = await _authService.RequestPasswordResetAsync(email);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.ResetPasswordAsync(model);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("is-verified/{userId}")]
    public async Task<IActionResult> IsVerified(int userId)
    {
        var result = await _authService.IsVerifiedAsync(userId);
        return result.Success && result.Data != null ? Ok(result) : BadRequest(result);
    }

    [HttpPost("verify-email/{userId}/{token}")]
    public async Task<IActionResult> VerifyEmail(int userId, string token)
    {
        var result = await _authService.VerifyEmailAsync(userId, token);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();

            var result = await _authService.LogoutAsync(HttpContext, token);
            return result.Success ? Ok(result.Data) : Unauthorized(result.Data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred.", exception = ex.Message });
        }
    }

    // This endpoint is for user service only
    //
    // TODO: Only Admin should be allowed to delete users
    [HttpDelete("delete/{userId}")]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        var result = await _authService.DeleteUserAsync(userId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [Authorize]
    [HttpGet("validate")]
    public IActionResult Validate()
    {
        var userId = User.FindFirst("userid")?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        if (userId == null)
            return Unauthorized(new ApiResult<bool>(false, false, "Invalid token."));

        return Ok(new ApiResult<bool>(true, true, "Token is valid."));
    }
}