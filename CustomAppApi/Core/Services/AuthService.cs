using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using CustomAppApi.Models.DTOs;
using CustomAppApi.Models.Entities;

namespace CustomAppApi.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public AuthService(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userService.GetByUsernameAsync(request.Username);
            
            if (request.Password != "test123")
                throw new InvalidOperationException("Invalid username or password");

            var token = await GenerateTokenAsync(user);
            var dealerId = user.UserType == UserType.Dealer ? 
                (await _userService.GetByIdAsync(user.Id))?.Id : null as int?;

            return new AuthResponse
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(
                    double.Parse(_configuration["JwtSettings:ExpirationInMinutes"])),
                UserType = user.UserType.ToString(),
                DealerId = dealerId
            };
        }

        public async Task<string> GenerateTokenAsync(UserDto user)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.UserType.ToString()),
                new Claim("UserId", user.Id.ToString())
            };

            if (user.UserType == UserType.Dealer)
            {
                var dealer = await _userService.GetByIdAsync(user.Id);
                if (dealer != null)
                {
                    claims.Add(new Claim("DealerId", dealer.Id.ToString()));
                }
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    double.Parse(_configuration["JwtSettings:ExpirationInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
} 