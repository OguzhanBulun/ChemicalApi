using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using CustomAppApi.Models.DTOs;
using CustomAppApi.Models.Entities;
using BCrypt.Net;
using AutoMapper;

namespace CustomAppApi.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthService(IUserService userService, IConfiguration configuration, IMapper mapper)
        {
            _userService = userService;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var userEntity = await _userService.GetByUsernameWithPasswordAsync(request.Username);
            
            if (!BCrypt.Net.BCrypt.Verify(request.Password, userEntity.PasswordHash))
                throw new InvalidOperationException("Invalid username or password");

            var userDto = _mapper.Map<UserDto>(userEntity);
            var token = await GenerateTokenAsync(userDto);
            var dealerId = userDto.UserType == UserType.Dealer ? 
                (await _userService.GetByIdAsync(userDto.Id!.Value))?.Id : null as int?;

            return new AuthResponse
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(
                    double.Parse(_configuration["JwtSettings:ExpirationInMinutes"])),
                UserType = userDto.UserType.ToString(),
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
                new Claim("UserId", user.Id!.Value.ToString())
            };

            if (user.UserType == UserType.Dealer)
            {
                var dealer = await _userService.GetByIdAsync(user.Id!.Value);
                if (dealer != null)
                {
                    claims.Add(new Claim("DealerId", dealer.Id!.Value.ToString()));
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