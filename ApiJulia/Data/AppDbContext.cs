
using ApiJulia.Models;
using Microsoft.EntityFrameworkCore;


public class AppDbContext : DbContext
{

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configura o preço do Produto
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)");

        // Configura o preço unitário no Item do Pedido
        modelBuilder.Entity<OrderItem>()
            .Property(p => p.UnitPrice)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Order>()
        .Property(o => o.Total)
        .HasColumnType("decimal(18,2)");
    }
}

