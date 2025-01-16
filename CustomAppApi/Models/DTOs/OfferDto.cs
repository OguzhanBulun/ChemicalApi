using CustomAppApi.Models.Entities;

namespace CustomAppApi.Models.DTOs
{
    public class OfferDto
    {
        public int Id { get; set; }
        public int DealerId { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public DateTime OfferDate { get; set; }
        public OfferStatus Status { get; set; }
        public DealerDto Dealer { get; set; }
        public ProductDto Product { get; set; }
    }
} 