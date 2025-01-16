using CustomAppApi.Models.DTOs;

namespace CustomAppApi.Core.Services
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class AuthResponse
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public string UserType { get; set; }
        public int? DealerId { get; set; }
    }

    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<string> GenerateTokenAsync(UserDto user);
    }
} 