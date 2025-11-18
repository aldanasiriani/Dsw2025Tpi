using Microsoft.EntityFrameworkCore;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain;


namespace Dsw2025Tpi.Data

{
    public class Dsw2025TpiContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public Dsw2025TpiContext(DbContextOptions<Dsw2025TpiContext> options)  : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Producto
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");

                entity.Property(p => p.Id)
                      .ValueGeneratedOnAdd();

                entity.Property(p => p.Sku);
                entity.HasIndex(p => p.Sku) // Crea un indice
                      .IsUnique();


                entity.Property(p => p.InternalCode);


                entity.Property(p => p.Name)
                      .HasMaxLength(30);


                entity.Property(p => p.Description)
                      .HasMaxLength(50);


                entity.Property(p => p.CurrentUnitPrice)
                      .HasPrecision(18, 2);


                entity.Property(p => p.StockQuantity);


                entity.Property(p => p.IsActive);

            });

            // Orders
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");


                entity.Property(o => o.Id)

                    .ValueGeneratedOnAdd();


                entity.Property(o => o.ShippingAddress)

                    .HasMaxLength(255);

                entity.Property(o => o.BillingAddress)

                    .HasMaxLength(255);

               /* entity.Property(o => o.Notes)

                    .HasMaxLength(500);*/


                entity.Property(o => o.Status);

                entity.Property(o => o.TotalAmount)
                      .IsRequired()
                      .HasPrecision(18, 2);


                
                entity.HasOne(o => o.Customer) 
                      .WithMany()            
                      .HasForeignKey(o => o.CustomerId) 
                      .IsRequired();


               
                entity.HasMany(o => o.OrderItems) 
                      .WithOne(oi => oi.Order)    
                      .HasForeignKey(oi => oi.OrderId) 
                      .IsRequired();
            });
            // OrderItem
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("OrderItems");


                entity.Property(oi => oi.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(oi => oi.Quantity)
                    .IsRequired();


               /* entity.Property(oi => oi.UnitPrice)
                    .IsRequired()
                    .HasPrecision(18, 2);*/

                // Subtotal 
               // entity.Property(oi => oi.Subtotal)
                   // .IsRequired()
                   // .HasPrecision(18, 2);

                
                entity.HasOne(oi => oi.Product) 
                      .WithMany()            
                      .HasForeignKey(oi => oi.ProductId) 
                      .IsRequired();           


            });

            // Customer
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customers");


                entity.Property(c => c.Id)
                    .ValueGeneratedOnAdd();


                entity.Property(c => c.Email)
                    .HasMaxLength(100);


                entity.Property(c => c.Name)
                    .HasMaxLength(100);


                entity.Property(c => c.PhoneNumber)
                    .HasMaxLength(20);
            });


        }



        }

    }

