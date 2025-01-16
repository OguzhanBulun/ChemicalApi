using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Dealer : BaseEntity
{
    public string CompanyName { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string TaxNumber { get; set; }
    
    // Navigation properties
    public int UserId { get; set; }
    public User User { get; set; }
    public ICollection<Offer> Offers { get; set; }
    public ICollection<Sale> Sales { get; set; }
} 