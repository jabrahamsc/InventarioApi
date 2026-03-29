using System;
using InventarioApi.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InventarioApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(150);
            entity.Property(p => p.Price).HasPrecision(18, 2);
            entity.Ignore(p => p.RequiresRestocking);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.Email).IsRequired().HasMaxLength(200);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Role).IsRequired().HasMaxLength(20);
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.TotalAmount).HasPrecision(18, 2);
            entity.HasOne(s => s.User)
                  .WithMany(u => u.Sales)
                  .HasForeignKey(s => s.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SaleItem>(entity =>
        {
            entity.HasKey(si => si.Id);
            entity.Property(si => si.UnitPrice).HasPrecision(18, 2);
            entity.Ignore(si => si.Subtotal); 
            entity.HasOne(si => si.Sale)
                  .WithMany(s => s.SaleItems)
                  .HasForeignKey(si => si.SaleId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(si => si.Product)
                  .WithMany(p => p.SaleItems)
                  .HasForeignKey(si => si.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@inventario.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = Roles.Admin,
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = 2,
                Username = "empleado",
                Email = "empleado@inventario.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Empleado123!"),
                Role = Roles.Employee,
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        //Se dejan estos productos de ejemplo
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Laptop Dell XPS 15", Price = 1200.00m, CurrentStock = 10, MinimumStock = 5, CreatedAt = DateTime.Now },
            new Product { Id = 2, Name = "Mouse Inalambrico Logitech", Price = 35.00m, CurrentStock = 50, MinimumStock = 20, CreatedAt = DateTime.Now },
            new Product { Id = 3, Name = "Teclado Mecanico Redragon", Price = 55.00m, CurrentStock = 3, MinimumStock = 10, CreatedAt = DateTime.Now },
            new Product { Id = 4, Name = "Monitor LG 27 pulgadas", Price = 320.00m, CurrentStock = 8, MinimumStock = 5, CreatedAt = DateTime.Now },
            new Product { Id = 5, Name = "Auriculares Sony WH-1000XM5", Price = 280.00m, CurrentStock = 2, MinimumStock = 8, CreatedAt = DateTime.Now },
            new Product { Id = 6, Name = "Laptop MSi THIN 10UE", Price = 800.00m, CurrentStock = 5, MinimumStock = 2, CreatedAt = DateTime.Now},
            new Product { Id = 7, Name = "Lenovo Thinkpad E14", Price = 400.00m, CurrentStock = 21, MinimumStock = 3, CreatedAt = DateTime.Now }
        );
    }
}