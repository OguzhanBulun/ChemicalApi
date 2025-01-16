using System;
using System.Collections.Generic;
using CustomAppApi.Models.Entities;

namespace CustomAppApi.Models.Entities
{
    public class Dealer : BaseEntity
    {
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string TaxNumber { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Offer> Offers { get; set; }
        public virtual ICollection<Sale> Sales { get; set; }
    }
} 