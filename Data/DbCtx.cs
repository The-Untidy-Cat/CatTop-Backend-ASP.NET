using asp.net.Models;
using Microsoft.EntityFrameworkCore;

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
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<AddressBook> AddressBooks { get; set; }
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
                entity.HasOne(e => e.Customer).WithOne(e => e.User).HasForeignKey<Customer>(e => e.UserId);
                entity.HasOne(e => e.Employee).WithOne(e => e.User).HasForeignKey<Employee>(e => e.UserId);
            });
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.PhoneNumber).IsUnique();
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.HasOne(e => e.User).WithOne(e => e.Customer);
            });
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });
                entity.HasOne(e => e.User).WithMany(e => e.UserRoles).HasForeignKey(e => e.UserId);
            });
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasOne(e => e.User).WithOne(e => e.Employee);
            });
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasOne(e => e.AddressBook).WithMany(e => e.Order).HasForeignKey(e => e.AddressId);
                entity.HasOne(e => e.Customer).WithMany(e => e.Order).HasForeignKey(e => e.CustomerId);
                entity.HasOne(e => e.Employee).WithMany(e => e.Order).HasForeignKey(e => e.EmployeeId);
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
        }
    }
}
