namespace CustomAppApi.Models.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserType UserType { get; set; }
        public Dealer? Dealer { get; set; }
    }
} 