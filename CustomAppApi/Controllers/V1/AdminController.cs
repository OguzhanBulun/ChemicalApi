using Microsoft.AspNetCore.Mvc;
using CustomAppApi.Core.Services;
using CustomAppApi.Models.DTOs;
using CustomAppApi.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;

namespace CustomAppApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IHostEnvironment _environment;

        public AdminController(IUserService userService, IHostEnvironment environment)
        {
            _userService = userService;
            _environment = environment;
        }

        [HttpPost("seed")]
        public async Task<IActionResult> SeedAdmin([FromBody] SeedAdminRequest request)
        {
            // Sadece Development ortamında çalışır
            if (!_environment.IsDevelopment())
            {
                return NotFound();
            }

            var adminDto = new UserDto
            {
                Username = request.Username ?? "admin",
                Email = request.Email ?? "admin@example.com",
                UserType = UserType.Admin
            };

            try
            {
                var createdAdmin = await _userService.CreateWithPasswordAsync(
                    adminDto, 
                    request.Password ?? "Admin123!", 
                    isSeedOperation: true);  // Seed işlemi olduğunu belirt
                return Ok(new { Message = "Admin user created successfully", User = createdAdmin });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }

    public class SeedAdminRequest
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
} 