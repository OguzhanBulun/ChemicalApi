namespace CustomAppApi.Models.DTOs
{
    public class SaleDto
    {
        public int Id { get; set; }
        public int DealerId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime SaleDate { get; set; }
        public DealerDto Dealer { get; set; }
        public ProductDto Product { get; set; }
    }
} 