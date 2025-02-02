using AutoMapper;
using CustomAppApi.Core.Repositories;
using CustomAppApi.Core.UnitOfWork;
using CustomAppApi.Models.DTOs;
using CustomAppApi.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using BCrypt.Net;

namespace CustomAppApi.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(
            IGenericRepository<User> userRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        private UserDto GetCurrentUser()
        {
            var username = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return null;

            var userTypeString = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
            if (Enum.TryParse<UserType>(userTypeString, out var userType))
            {
                return new UserDto
                {
                    Username = username,
                    UserType = userType
                };
            }

            return null;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAll().ToListAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found.");
                
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> CreateAsync(UserDto userDto)
        {
            if (await ExistsAsync(userDto.Username, userDto.Email))
                throw new InvalidOperationException("Username or email already exists.");

            var user = _mapper.Map<User>(userDto);
            user.UserType = userDto.UserType;

            if (userDto.UserType == UserType.Admin)
            {
                var currentUser = GetCurrentUser();
                if (currentUser?.UserType != UserType.Admin)
                    throw new UnauthorizedAccessException("Only admins can create admin users");
            }

            await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();
            
            return _mapper.Map<UserDto>(user);
        }

        public async Task UpdateAsync(UserDto userDto)
        {
            var existingUser = await _userRepository.GetByIdAsync(userDto.Id.Value);
            if (existingUser == null)
                throw new KeyNotFoundException($"User with ID {userDto.Id} not found.");

            existingUser.UserType = userDto.UserType;

            _mapper.Map(userDto, existingUser);
            _userRepository.Update(existingUser);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found.");

            _userRepository.Remove(user);
            await _unitOfWork.CommitAsync();
        }

        public async Task<UserDto> GetByUsernameAsync(string username)
        {
            var user = await _userRepository.Where(u => u.Username == username).FirstOrDefaultAsync();
            if (user == null)
                throw new KeyNotFoundException($"User with username {username} not found.");
                
            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> ExistsAsync(string username, string email)
        {
            return await _userRepository.AnyAsync(u => 
                u.Username == username || u.Email == email);
        }

        public async Task ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found.");

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
                throw new InvalidOperationException("Current password is incorrect");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            
            _userRepository.Update(user);
            await _unitOfWork.CommitAsync();
        }

        public async Task<User> GetByUsernameWithPasswordAsync(string username)
        {
            var user = await _userRepository.Where(u => u.Username == username).FirstOrDefaultAsync();
            if (user == null)
                throw new KeyNotFoundException($"User with username {username} not found.");
                
            return user;
        }

        public async Task<UserDto> CreateWithPasswordAsync(UserDto userDto, string password, bool isSeedOperation = false)
        {
            var exists = await ExistsAsync(userDto.Username, userDto.Email);
            if (exists)
                throw new InvalidOperationException("Username or email already exists.");

            if (userDto.UserType == 0)
                userDto.UserType = UserType.Personnel;

            if (!isSeedOperation && userDto.UserType == UserType.Admin)
            {
                var currentUser = GetCurrentUser();
                if (currentUser?.UserType != UserType.Admin)
                    throw new UnauthorizedAccessException("Only admins can create admin users");
            }

            var user = _mapper.Map<User>(userDto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            
            await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();
            
            return _mapper.Map<UserDto>(user);
        }
    }
} 