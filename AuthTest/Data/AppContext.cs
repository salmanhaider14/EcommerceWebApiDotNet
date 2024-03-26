using DotNetEcommerceAPI.Entitities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

public class AppContext : IdentityDbContext<IdentityUser>
{
    public AppContext(DbContextOptions<AppContext> options) : base(options)
    {

    }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OrderItem>()
            .HasKey(oi => oi.OrderItemId); 

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId);
        SeedData(modelBuilder);

    }
    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed categories
        modelBuilder.Entity<Category>().HasData(
            new Category { CategoryId = 1, Name = "Electronics" },
            new Category { CategoryId = 2, Name = "Clothing" },
            new Category { CategoryId = 3, Name = "Books" },
            new Category { CategoryId = 4, Name = "Furniture" },
            new Category { CategoryId = 5, Name = "Sports Equipment" }
        );

        // Seed products
        modelBuilder.Entity<Product>().HasData(
            new Product { ProductId = 1, Name = "Smartphone", Description = "High-end smartphone", Price = 999.99m, CategoryId = 1 },
            new Product { ProductId = 2, Name = "T-shirt", Description = "Casual t-shirt", Price = 19.99m, CategoryId = 2 },
            new Product { ProductId = 3, Name = "Java Programming Book", Description = "Learn Java programming", Price = 29.99m, CategoryId = 3 },
            new Product { ProductId = 4, Name = "Sofa", Description = "Comfortable sofa", Price = 499.99m, CategoryId = 4 },
            new Product { ProductId = 5, Name = "Basketball", Description = "Official size basketball", Price = 29.99m, CategoryId = 5 }
        );
    }

}