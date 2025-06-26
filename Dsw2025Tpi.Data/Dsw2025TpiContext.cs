using Microsoft.EntityFrameworkCore;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain;


namespace Dsw2025Tpi.Data

{
public class Dsw2025TpiContext: DbContext
{

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Product>()
         .ToTable("Products")
         .Property(p => p.Id)
         .ValueGeneratedOnAdd();

            modelBuilder.Entity<Product>()
           //.Property(p => p.Sku)
           .HasIndex(p => p.Sku)
           .IsUnique();  // índice únic
           //.HasMaxLength(20);
     

            modelBuilder.Entity<Product>()
        .Property(p => p.InternalCode);

        modelBuilder.Entity<Product>()
        .Property(p => p.Name)
         .HasMaxLength(20);

        modelBuilder.Entity<Product>()
        .Property(p => p.Description)
         .HasMaxLength(50);

        modelBuilder.Entity<Product>()
        .Property(p => p.CurrentUnitPrice)
        .HasPrecision(18, 2);

        modelBuilder.Entity<Product>()
        .Property(p => p.StockQuantity);

        modelBuilder.Entity<Product>()
        .Property(p => p.IsActive);



         modelBuilder.Entity<Order>()
         .ToTable("Orders")
         .Property(p => p.Id)
         .ValueGeneratedOnAdd();

        modelBuilder.Entity<Order>()
        .Property(p => p.Date);

        modelBuilder.Entity<Order>()
      .Property(p => p.ShippingAddress);

        modelBuilder.Entity<Order>()
      .Property(p => p.BillingAddress);

        modelBuilder.Entity<Order>()
      .Property(p => p.Notes);

        modelBuilder.Entity<Order>()
     .Property(p => p.TotalAmount);


        modelBuilder.Entity<OrderItem>()
        .ToTable("OrderItems")
        .Property(p => p.Id)
        .ValueGeneratedOnAdd();

        modelBuilder.Entity<OrderItem>()
        .Property(p => p.Quantity);

        modelBuilder.Entity<OrderItem>()
        .Property(p => p.UnitPrice);

        modelBuilder.Entity<OrderItem>()
      .Property(p => p.Subtotal); 


        modelBuilder.Entity<Customer>()
        .ToTable("Customers")
        .Property(p => p.Id)
        .ValueGeneratedOnAdd();

        modelBuilder.Entity<Customer>()
       .Property(p => p.Email);

        modelBuilder.Entity<Customer>()
      .Property(p => p.Name);

        modelBuilder.Entity<Customer>()
      .Property(p => p.PhoneNumber);
    }


}

}