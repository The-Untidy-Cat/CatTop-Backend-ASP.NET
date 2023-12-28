using asp.net.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace asp.net.Data
{
    public class DbCtx : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderHistories> OrderHistories { get; set; }
        public DbSet<OrderItems> OrderItems { get; set; }
        public DbSet<ProductVariants> ProductVariants { get; set; }
        public DbSet<AddressBook> AddressBooks { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        public DbCtx(DbContextOptions<DbCtx> options) : base(options)
        {
            Database.EnsureCreated();
        }
        public DbCtx()
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasOne(u => u.Customer).WithOne(c => c.User);
            });
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.PhoneNumber).IsUnique();
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.Property(e => e.UserId).IsRequired();
                entity.HasOne(c => c.User).WithOne(u => u.Customer).HasForeignKey<Customer>(c => c.UserId);
            });
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });
                entity.HasOne(e => e.User).WithMany(e => e.UserRoles).HasForeignKey(e => e.UserId);
            });
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasOne(e => e.User).WithOne(e => e.Employee);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.PhoneNumber).IsUnique();
                entity.HasIndex(e => e.UserId).IsUnique();
            });
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasOne(e => e.AddressBook).WithMany(e => e.Orders).HasForeignKey(e => e.AddressId);
                entity.HasOne(e => e.Customer).WithMany(e => e.Orders).HasForeignKey(e => e.CustomerId);
                entity.HasOne(e => e.Employee).WithMany(e => e.Orders).HasForeignKey(e => e.EmployeeId);
            });
            modelBuilder.Entity<OrderHistories>(entity =>
            {
                entity.HasOne(e => e.Order).WithMany(e => e.OrderHistories).HasForeignKey(e => e.OrderId);
            });
            modelBuilder.Entity<OrderItems>(entity =>
            {
                entity.HasOne(e => e.Order).WithMany(e => e.OrderItems).HasForeignKey(e => e.OrderId);
                entity.HasOne(e => e.ProductVariant).WithMany(e => e.OrderItems).HasForeignKey(e => e.VariantId);
            });
            modelBuilder.Entity<Brand>(entity =>
            {
                entity.HasIndex(e => e.Id).IsUnique();
            });
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.Id).IsUnique();
                entity.HasOne(e => e.Brand).WithMany(e => e.Products).HasForeignKey(e => e.BrandId);
            });
            modelBuilder.Entity<ProductVariants>(entity =>
            {
                entity.HasOne(e => e.Product).WithMany(e => e.ProductVariants).HasForeignKey(e => e.ProductID);
            });
            modelBuilder.Entity<Brand>(entity =>
            {
                //entity.HasOne(e => e.brandPa).WithMany(e => e.brandMain).HasForeignKey(e => e.parent_id);
            });
            modelBuilder.Entity<AddressBook>(entity =>
            {
                entity.HasOne(c => c.Customer).WithOne(a => a.AddressBooks).HasForeignKey<AddressBook>(a => a.CustomerId);
            });
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasOne(c => c.Customer).WithOne(c => c.Cart).HasForeignKey<Cart>(c => c.CustomerID);
                entity.HasOne(v => v.Variant).WithOne(c => c.Cart).HasForeignKey<Cart>(c => c.VariantId);
            });
            //modelBuilder.Entity<PasswordResetToken>(entity =>
            //{
            //});
        }
    }
}
