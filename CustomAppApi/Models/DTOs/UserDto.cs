using CustomAppApi.Models.Entities;

namespace CustomAppApi.Models.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public UserType UserType { get; set; }
    }
} 