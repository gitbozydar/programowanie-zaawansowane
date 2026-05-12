using Microsoft.EntityFrameworkCore;

public class OrderFlowContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
        .UseSqlite("Data Source=orderflow.db")
        .LogTo(Console.WriteLine);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
            .HasOne(i => i.Product)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .Ignore(o => o.TotalPrice);

        modelBuilder.Entity<OrderItem>()
            .Ignore(i => i.TotalPrice);

        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<OrderItem>()
            .Property(i => i.UnitPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.Username);

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.Status);
    }
}