using Microsoft.AspNetCore.Mvc;
using CustomAppApi.Core.Services;
using CustomAppApi.Models.DTOs;
using CustomAppApi.Models.Common;
using Microsoft.AspNetCore.RateLimiting;

namespace CustomAppApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }

        [HttpPost("register")]
        [EnableRateLimiting("RegisterLimit")]
        public async Task<ActionResult<ApiResponse<UserDto>>> Register(RegisterRequest request)
        {
            try 
            {
                var userDto = new UserDto
                {
                    Username = request.Username.Trim().ToLower(),
                    Email = request.Email.Trim().ToLower(),
                    UserType = request.UserType
                };

                var user = await _userService.CreateWithPasswordAsync(userDto, request.Password);
                return CreatedAtAction(
                    nameof(UserController.GetById), 
                    "User",
                    new { id = user.Id }, 
                    ApiResponse<UserDto>.SuccessResult(user, "User registered successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<UserDto>.ErrorResult(ex.Message));
            }
        }
    }
} 