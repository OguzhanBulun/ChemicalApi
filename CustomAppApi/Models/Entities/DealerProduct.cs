namespace CustomAppApi.Models.Entities
{
    public class DealerProduct : BaseEntity
    {
        public int DealerId { get; set; }
        public int ProductId { get; set; }
        public Dealer Dealer { get; set; }
        public Product Product { get; set; }
    }
} 