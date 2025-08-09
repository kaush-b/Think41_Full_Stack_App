using HRBMS.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRBMS.Infrastructure.Data;

public class HrbmsDbContext : DbContext
{
    public HrbmsDbContext(DbContextOptions<HrbmsDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(200);
            entity.Property(u => u.PasswordHash).IsRequired();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.HasIndex(r => r.Name).IsUnique();
            entity.Property(r => r.Name).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(ur => ur.Id);
            entity.HasIndex(ur => new { ur.UserId, ur.RoleId }).IsUnique();
            entity.HasOne(ur => ur.User).WithMany(u => u.UserRoles).HasForeignKey(ur => ur.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(ur => ur.Role).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.RoleId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.HasIndex(r => r.RoomNumber).IsUnique();
            entity.Property(r => r.RoomNumber).IsRequired().HasMaxLength(20);
            entity.Property(r => r.PricePerNight).HasColumnType("decimal(10,2)");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.HasOne(b => b.Room).WithMany(r => r.Bookings).HasForeignKey(b => b.RoomId);
            entity.HasOne(b => b.Guest).WithMany(u => u.Bookings).HasForeignKey(b => b.GuestId).OnDelete(DeleteBehavior.Restrict);
            entity.Property(b => b.Status).HasConversion<int>();
            entity.HasIndex(b => new { b.RoomId, b.CheckIn, b.CheckOut });
        });
    }
}