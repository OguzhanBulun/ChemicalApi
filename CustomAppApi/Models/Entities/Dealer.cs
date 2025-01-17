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
        public int? UserId { get; set; }
        public User AssignedUser { get; set; }
        public int? CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; }
        public virtual ICollection<DealerProduct> DealerProducts { get; set; }
        public virtual ICollection<Offer> Offers { get; set; }
        public virtual ICollection<Sale> Sales { get; set; }
    }
} 