using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StockFlow360.Domain.Entities;

namespace StockFlow360.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets for domain entities
        public DbSet<Product> Products { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleDetail> SaleDetails { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseDetail> PurchaseDetails { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        // Optional: Fluent API configuration
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Product - Inventory (1-to-1)
            builder.Entity<Product>()
                .HasOne<Inventory>()
                .WithOne(i => i.Product)
                .HasForeignKey<Inventory>(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Sale - SaleDetails (1-to-many)
            builder.Entity<Sale>()
                .HasMany(s => s.Items)
                .WithOne()
                .HasForeignKey(sd => sd.SaleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Purchase - PurchaseDetails (1-to-many)
            builder.Entity<Purchase>()
                .HasMany(p => p.Items)
                .WithOne()
                .HasForeignKey(pd => pd.PurchaseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Supplier - Products (1-to-many)
            builder.Entity<Supplier>()
                .HasMany<Product>()
                .WithOne(p => p.Supplier)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product - SaleDetail (many-to-1)
            builder.Entity<SaleDetail>()
                .HasOne<Product>()
                .WithMany()
                .HasForeignKey(sd => sd.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product - PurchaseDetail (many-to-1)
            builder.Entity<PurchaseDetail>()
                .HasOne<Product>()
                .WithMany()
                .HasForeignKey(pd => pd.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Optional: Composite keys for SaleDetail and PurchaseDetail
            builder.Entity<SaleDetail>()
                .HasKey(sd => new { sd.SaleId, sd.ProductId });

            builder.Entity<PurchaseDetail>()
                .HasKey(pd => new { pd.PurchaseId, pd.ProductId });
        }

    }
}
