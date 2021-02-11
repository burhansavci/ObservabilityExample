using Microsoft.EntityFrameworkCore;

namespace ObservabilityExample.Services.Customers.Domain
{
    public class CustomerContext : DbContext
    {
        public CustomerContext(DbContextOptions<CustomerContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }

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
        }
    }
}