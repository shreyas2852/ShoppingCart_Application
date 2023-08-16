using Microsoft.EntityFrameworkCore;
using ShoppingCart_6.Models;
using System.Collections.Generic;

namespace ShoppingCart_6.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Register> Registers { get; set; }
        public DbSet<Order> Orders { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Register) 
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.RegisterId); 
            
            modelBuilder.Entity<Order>()
               .HasOne(o => o.Product)
               .WithMany(u => u.Orders)
               .HasForeignKey(o => o.ProductId);

            modelBuilder.Entity<Product>()
                .Property(o => o.Id).UseIdentityColumn();
                
        }
    }
}
