using Microsoft.EntityFrameworkCore;
using CustomAppApi.Models.Entities;

namespace CustomAppApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Dealer> Dealers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<DealerProduct> DealerProducts { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Offer>()
                .Property(o => o.Price)
                .HasPrecision(18, 2);
            
            modelBuilder.Entity<Offer>()
                .Property(o => o.Quantity)
                .HasPrecision(18, 2);
            
            modelBuilder.Entity<Sale>()
                .Property(s => s.Quantity)
                .HasPrecision(18, 2);
            
            modelBuilder.Entity<Sale>()
                .Property(s => s.UnitPrice)
                .HasPrecision(18, 2);
            
            modelBuilder.Entity<Sale>()
                .Property(s => s.TotalPrice)
                .HasPrecision(18, 2);
            
            modelBuilder.Entity<Offer>()
                .HasOne(o => o.Dealer)
                .WithMany(d => d.Offers)
                .HasForeignKey(o => o.DealerId);
            
            modelBuilder.Entity<Offer>()
                .HasOne(o => o.Product)
                .WithMany(p => p.Offers)
                .HasForeignKey(o => o.ProductId);
            
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Dealer)
                .WithMany(d => d.Sales)
                .HasForeignKey(s => s.DealerId);
            
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Product)
                .WithMany(p => p.Sales)
                .HasForeignKey(s => s.ProductId);
            
            modelBuilder.Entity<DealerProduct>()
                .HasOne(dp => dp.Dealer)
                .WithMany(d => d.DealerProducts)
                .HasForeignKey(dp => dp.DealerId);
            
            modelBuilder.Entity<DealerProduct>()
                .HasOne(dp => dp.Product)
                .WithMany(p => p.DealerProducts)
                .HasForeignKey(dp => dp.ProductId);
            
            modelBuilder.Entity<Dealer>()
                .HasOne(d => d.AssignedUser)
                .WithMany(u => u.AssignedDealers)
                .HasForeignKey(d => d.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Dealer>()
                .HasOne(d => d.CreatedByUser)
                .WithMany(u => u.CreatedDealers)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 