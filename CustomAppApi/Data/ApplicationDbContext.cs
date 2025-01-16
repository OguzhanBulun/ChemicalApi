using Microsoft.EntityFrameworkCore;
using Models.Entities;

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
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // İlişkileri tanımlayalım
        modelBuilder.Entity<Dealer>()
            .HasOne(d => d.User)
            .WithOne(u => u.Dealer)
            .HasForeignKey<Dealer>(d => d.UserId);
            
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
    }
} 