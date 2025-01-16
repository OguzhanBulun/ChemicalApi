using CustomAppApi.Models.DTOs;

namespace CustomAppApi.Core.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto> GetByIdAsync(int id);
        Task<UserDto> CreateAsync(UserDto userDto);
        Task UpdateAsync(UserDto userDto);
        Task DeleteAsync(int id);
        Task<UserDto> GetByUsernameAsync(string username);
        Task<bool> ExistsAsync(string username, string email);
    }
} 