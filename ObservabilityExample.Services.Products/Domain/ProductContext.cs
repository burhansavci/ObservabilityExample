using Microsoft.EntityFrameworkCore;

namespace ObservabilityExample.Services.Products.Domain
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(p =>
            {
                p.Property(x => x.Id).ValueGeneratedNever();
                p.Property(x => x.Name);
                p.Property(x => x.Description);
                p.Property(x => x.Vendor);
                p.Property(x => x.Price).HasColumnType("decimal(18, 2)");;
                p.Property(x => x.Quantity);
            });
        }
    }
}