using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Product : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Unit { get; set; }
    
    // Navigation properties
    public ICollection<Offer> Offers { get; set; }
    public ICollection<Sale> Sales { get; set; }
} 