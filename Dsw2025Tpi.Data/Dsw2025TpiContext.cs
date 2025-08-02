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
                entity.Property(o => o.Notes)

                    .HasMaxLength(500);


                entity.Property(o => o.Status);

                entity.Property(o => o.TotalAmount)
                      .IsRequired()
                      .HasPrecision(18, 2);


                // Relación Order (Many) a Customer (One)
                // Una orden tiene un CustomerId (FK) y una propiedad de navegación Customer
                entity.HasOne(o => o.Customer) // Una Order tiene un Customer
                      .WithMany()            // Un Customer puede tener muchas Orders (sin propiedad de navegación inversa en Customer)
                      .HasForeignKey(o => o.CustomerId) // La propiedad de clave foránea en Order
                      .IsRequired();


                // Relación Order (One) a OrderItem (Many)
                // Una orden tiene una colección de OrderItems, y cada OrderItem tiene una FK a Order
                entity.HasMany(o => o.OrderItems) // Una Order tiene muchos OrderItems
                      .WithOne(oi => oi.Order)     // Un OrderItem tiene una Order
                      .HasForeignKey(oi => oi.OrderId) // La propiedad de clave foránea en OrderItem
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


                entity.Property(oi => oi.UnitPrice)
                    .IsRequired()
                    .HasPrecision(18, 2);

                // Subtotal (obligatorio, precisión para decimales, ahora persistible)
               // entity.Property(oi => oi.Subtotal)
                   // .IsRequired()
                   // .HasPrecision(18, 2);

                // Relación OrderItem (Many) a Product (One)
                entity.HasOne(oi => oi.Product) // Un OrderItem tiene un Product
                      .WithMany()              // Un Product puede estar en muchos OrderItems (sin propiedad de navegación inversa en Product)
                      .HasForeignKey(oi => oi.ProductId) // La propiedad de clave foránea en OrderItem
                      .IsRequired();           // Un OrderItem debe tener un Product


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

