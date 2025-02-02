using System;
using CustomAppApi.Models.Entities;

namespace CustomAppApi.Models.Entities
{
    public class Sale : BaseEntity
    {
        public int DealerId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime SaleDate { get; set; }
        public Dealer Dealer { get; set; }
        public Product Product { get; set; }
    }
} 