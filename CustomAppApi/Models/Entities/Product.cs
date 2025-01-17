using System;
using System.Collections.Generic;
using CustomAppApi.Models.Entities;

namespace CustomAppApi.Models.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public virtual ICollection<DealerProduct> DealerProducts { get; set; }
        public virtual ICollection<Offer> Offers { get; set; }
        public virtual ICollection<Sale> Sales { get; set; }
    }
} 