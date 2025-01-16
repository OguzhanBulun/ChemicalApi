using Microsoft.AspNetCore.Mvc;
using CustomAppApi.Core.Services;
using CustomAppApi.Models.DTOs;
using CustomAppApi.Models.Common;
using Microsoft.AspNetCore.Authorization;

namespace CustomAppApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResult(users));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            return Ok(ApiResponse<UserDto>.SuccessResult(user));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserDto>>> Create(UserDto userDto)
        {
            var createdUser = await _userService.CreateAsync(userDto);
            return CreatedAtAction(nameof(GetById), 
                new { id = createdUser.Id }, 
                ApiResponse<UserDto>.SuccessResult(createdUser, "Kullanıcı başarıyla oluşturuldu"));
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> ChangePassword(ChangePasswordRequest request)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            await _userService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
            
            return Ok(ApiResponse<object>.SuccessResult(null, "Password changed successfully"));
        }

    }
} 