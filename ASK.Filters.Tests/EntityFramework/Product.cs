using Microsoft.EntityFrameworkCore;

namespace ASK.Filters.Tests.EntityFramework;

public record Address(int Id, string AddressLine1, string AddressLine2, string City, string State, string Country);

public class Product
{
    public int Id { get; private set; }
    public string? Name { get; private set; }
    public decimal Price { get; private set; }
    public bool IsOutOfStock { get; private set; }
    public DateTime CreationDate { get; private set; }
    public ICollection<Address> Addresses { get; private  set; } = new List<Address>();

    public Product(){}

    public Product(int id, string? name, decimal price, bool isOutOfStock, DateTime creationDate, ICollection<Address> addresses)
    {
        Id = id;
        Name = name;
        Price = price;
        IsOutOfStock = isOutOfStock;
        CreationDate = creationDate;
        Addresses = addresses;
    }

    public static Product[] SampleValues =
    [
        new Product(1, "Laptop Pro", 999.99m, false, new DateTime(2023, 1, 15), [
            new Address(1, "Rue", "Line2", "Paris", "State", "France"),
            new Address(2, "Rue", "Line2", "Brussels", "Liege", "Belgium"),]),
        new Product(2, "Smartphone X", 799.49m, true, new DateTime(2023, 2, 20), [
            new Address(3, "Rue", "Line2", "Brussels", "State", "Country")]),
        new Product(3, "Wireless Earbuds", 129.99m, false, new DateTime(2023, 3, 5), [
            new Address(4, "Rue", "Line2", "Brussels", "State", "Country"),
            new Address(5, "Rue", "Line2", "Amsterdam", "State", "Country"),]),
        new Product(4, "Gaming Console", 499.99m, true, new DateTime(2023, 4, 10), [
            new Address(6,"Rue", "Line2", "Paris", "State", "Country"),]),
        new Product(5, "4K TV", 1199.99m, false, new DateTime(2023, 5, 25), [
            new Address(7,"Rue", "Line2", "Paris", "State", "Country"),]),
        new Product(6, "Smartwatch", 199.99m, true, new DateTime(2023, 6, 30), []),
        new Product(7, "Bluetooth Speaker", 89.99m, false, new DateTime(2023, 7, 15), []),
        new Product(8, "Tablet Pro", 599.99m, true, new DateTime(2023, 8, 20), []),
        new Product(9, "Digital Camera", 749.99m, false, new DateTime(2023, 9, 1), []),
        new Product(10, null, 149.99m, true, new DateTime(2023, 10, 10), []),
        new Product(11, "E-Reader", 129.99m, false, new DateTime(2023, 11, 25), []),
        new Product(12, null, 399.99m, true, new DateTime(2023, 12, 30), []),
        new Product(13, "Drone", 899.99m, false, new DateTime(2024, 1, 15), []),
        new Product(14, "Portable Charger", 49.99m, true, new DateTime(2024, 2, 20), []),
        new Product(15, "Smart Home Hub", 199.99m, false, new DateTime(2024, 3, 5), []),
        new Product(16, "Noise Cancelling Headphones", 299.99m, true, new DateTime(2024, 4, 10), []),
        new Product(17, "Electric Scooter", 499.99m, false, new DateTime(2024, 5, 25), []),
        new Product(18, "Action Camera", 249.99m, true, new DateTime(2024, 6, 30), []),
        new Product(19, "Smart Light Bulb", 29.99m, false, new DateTime(2024, 7, 15), []),
        new Product(20, "Robot Vacuum", 399.99m, true, new DateTime(2024, 8, 20), []),
        new Product(21, "3D Printer", 999.99m, false, new DateTime(2024, 9, 5), []),
        new Product(22, "Electric Toothbrush", 79.99m, true, new DateTime(2023, 12, 30), []),
        new Product(23, "Smart Thermostat", 199.99m, false, new DateTime(2024, 11, 25), []),
        new Product(24, "Security Camera", 149.99m, true, new DateTime(2023, 12, 30), []),
        new Product(25, "Wireless Charger", 39.99m, false, new DateTime(2025, 1, 15), []),
        new Product(26, "", 390.99m, true, new DateTime(2045, 1, 15), [])
    ];
}

public class ProductDbContext(DbContextOptions<ProductDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(x =>
            {
                x.ToTable("addresses");
                x.Property(y => y.Country);
                x.Property(y => y.City);
                x.Property(y => y.Id);
                x.Property(y => y.AddressLine1);
                x.Property(y => y.AddressLine2);
                x.Property(y => y.Country);
                x.Property("ProductId");
            }
        );

        modelBuilder.Entity<Product>()
                    .HasMany<Address>(x => x.Addresses);

        modelBuilder.Entity<Product>()
                    .Navigation(x => x.Addresses).AutoInclude(false);
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Address> Addresses { get; set; }
}