using Microsoft.AspNetCore.Mvc;
using CustomAppApi.Core.Services;
using CustomAppApi.Models.DTOs;
using CustomAppApi.Models.Common;

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

    }
} 