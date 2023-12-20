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

            });
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.PhoneNumber).IsUnique();
                entity.HasIndex(e => e.UserId).IsUnique();
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
            });
        }
    }
}
