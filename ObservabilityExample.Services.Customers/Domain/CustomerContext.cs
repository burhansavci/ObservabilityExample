using Microsoft.EntityFrameworkCore;

namespace ObservabilityExample.Services.Customers.Domain
{
    public class CustomerContext : DbContext
    {
        public CustomerContext(DbContextOptions<CustomerContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(c =>
            {
                c.Property(x => x.Id).ValueGeneratedNever();
                c.Property(x => x.Email);
                c.Property(x => x.FirstName);
                c.Property(x => x.LastName);
                c.Property(x => x.Address);
                c.Property(x => x.Country);
                c.Property(x => x.CreatedAt);
            });
            
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