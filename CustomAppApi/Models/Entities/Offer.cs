using System;
using System.Collections.Generic;

public class Offer : BaseEntity
{
    public int DealerId { get; set; }
    public int ProductId { get; set; }
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public DateTime OfferDate { get; set; }
    public OfferStatus Status { get; set; }
    
    // Navigation properties
    public Dealer Dealer { get; set; }
    public Product Product { get; set; }
}

public enum OfferStatus
{
    Pending,
    Accepted,
    Rejected
} 