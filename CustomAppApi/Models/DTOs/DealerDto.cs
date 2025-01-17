namespace CustomAppApi.Models.DTOs
{
    public class DealerDto
    {
        public int? Id { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string TaxNumber { get; set; }
        public int? UserId { get; set; }
        public UserDto AssignedUser { get; set; }
        public int? CreatedByUserId { get; set; }
        public List<ProductDto> Products { get; set; }
    }
} 