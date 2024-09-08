using Microsoft.EntityFrameworkCore;

namespace ASK.Filters.Tests.EntityFramework;

public record Product(int Id, string Name, decimal Price, bool IsOutOfStock, DateTime CreationDate)
{
    public static Product[] SampleValues =
    [
        new Product(1, "Laptop Pro", 999.99m, false, new DateTime(2023, 1, 15)),
        new Product(2, "Smartphone X", 799.49m, true, new DateTime(2023, 2, 20)),
        new Product(3, "Wireless Earbuds", 129.99m, false, new DateTime(2023, 3, 5)),
        new Product(4, "Gaming Console", 499.99m, true, new DateTime(2023, 4, 10)),
        new Product(5, "4K TV", 1199.99m, false, new DateTime(2023, 5, 25)),
        new Product(6, "Smartwatch", 199.99m, true, new DateTime(2023, 6, 30)),
        new Product(7, "Bluetooth Speaker", 89.99m, false, new DateTime(2023, 7, 15)),
        new Product(8, "Tablet Pro", 599.99m, true, new DateTime(2023, 8, 20)),
        new Product(9, "Digital Camera", 749.99m, false, new DateTime(2023, 9, 1)),
        new Product(10, "Fitness Tracker", 149.99m, true, new DateTime(2023, 10, 10)),
        new Product(11, "E-Reader", 129.99m, false, new DateTime(2023, 11, 25)),
        new Product(12, "VR Headset", 399.99m, true, new DateTime(2023, 12, 30)),
        new Product(13, "Drone", 899.99m, false, new DateTime(2024, 1, 15)),
        new Product(14, "Portable Charger", 49.99m, true, new DateTime(2024, 2, 20)),
        new Product(15, "Smart Home Hub", 199.99m, false, new DateTime(2024, 3, 5)),
        new Product(16, "Noise Cancelling Headphones", 299.99m, true, new DateTime(2024, 4, 10)),
        new Product(17, "Electric Scooter", 499.99m, false, new DateTime(2024, 5, 25)),
        new Product(18, "Action Camera", 249.99m, true, new DateTime(2024, 6, 30)),
        new Product(19, "Smart Light Bulb", 29.99m, false, new DateTime(2024, 7, 15)),
        new Product(20, "Robot Vacuum", 399.99m, true, new DateTime(2024, 8, 20)),
        new Product(21, "3D Printer", 999.99m, false, new DateTime(2024, 9, 5)),
        new Product(22, "Electric Toothbrush", 79.99m, true, new DateTime(2023, 12, 30)),
        new Product(23, "Smart Thermostat", 199.99m, false, new DateTime(2024, 11, 25)),
        new Product(24, "Security Camera", 149.99m, true, new DateTime(2023, 12, 30)),
        new Product(25, "Wireless Charger", 39.99m, false, new DateTime(2025, 1, 15))
    ];
}

public class ProductDbContext(DbContextOptions<ProductDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>();
    }

    public DbSet<Product> Products { get; set; }
}