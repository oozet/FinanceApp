using FinanceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // public AppDbContext(DbContextOptions<AppDbContext> options)
    //     : base(options) { }

    public DbSet<User> Users { get; set; } = null!;

    //public DbSet<TransactionData> TransactionData { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Username).HasMaxLength(50).HasColumnName("username");

            // Shadow properties.
            entity.Property<string>("PasswordHash").HasColumnName("password_hash").IsRequired();
            entity.Property<string>("Salt").HasColumnName("salt").IsRequired();
        });
        // These ^v are the same.
        //modelBuilder.Entity<TransactionData>().ToTable("transactions");

        // Examples:
        // modelBuilder.Entity<User>()
        // .HasMany(u => u.Orders)
        // .WithOne(o => o.User)
        // .HasForeignKey(o => o.UserId);
        //
        // modelBuilder.Entity<Item>(entity =>
        // {
        //     entity.ToTable("item", "account");entity.Property(e => e.Id)
        //         .HasColumnName("id")
        //         .HasDefaultValueSql("nextval('account.item_id_seq'::regclass)");entity.Property(e => e.Description).HasColumnName("description");entity.Property(e => e.Name)
        //         .IsRequired()
        //         .HasColumnName("name");
        // });modelBuilder.HasSequence("item_id_seq", "account");
    }
}
