// using System;
// using System.Collections.Generic;
// using Microsoft.EntityFrameworkCore;

// namespace FinanceApp.Models;

// public partial class ProjectContext : DbContext
// {
//     public ProjectContext()
//     {
//     }

//     public ProjectContext(DbContextOptions<ProjectContext> options)
//         : base(options)
//     {
//     }

//     public virtual DbSet<Account> Accounts { get; set; }

//     public virtual DbSet<Person> Persons { get; set; }

//     public virtual DbSet<Transaction> Transactions { get; set; }

//     public virtual DbSet<User> Users { get; set; }

//     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
// #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//         => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=password;Database=project");

//     protected override void OnModelCreating(ModelBuilder modelBuilder)
//     {
//         modelBuilder
//             .HasPostgresEnum("account_type", new[] { "Private", "Savings", "Business" })
//             .HasPostgresEnum("transaction_type", new[] { "Deposit", "Withdrawal" })
//             .HasPostgresExtension("uuid-ossp");

//         modelBuilder.Entity<Account>(entity =>
//         {
//             entity.HasKey(e => e.AccountNumber).HasName("accounts_pkey");

//             entity.ToTable("accounts");

//             entity.Property(e => e.AccountNumber)
//                 .ValueGeneratedNever()
//                 .HasColumnName("account_number");
//             entity.Property(e => e.BalanceMinorUnit)
//                 .HasDefaultValue(0L)
//                 .HasColumnName("balance_minor_unit");
//             entity.Property(e => e.CreatedAt)
//                 .HasDefaultValueSql("CURRENT_TIMESTAMP")
//                 .HasColumnType("timestamp without time zone")
//                 .HasColumnName("created_at");
//             entity.Property(e => e.DeletedAt)
//                 .HasColumnType("timestamp without time zone")
//                 .HasColumnName("deleted_at");
//             entity.Property(e => e.UserId).HasColumnName("user_id");

//             entity.HasOne(d => d.User).WithMany(p => p.Accounts)
//                 .HasForeignKey(d => d.UserId)
//                 .HasConstraintName("accounts_user_id_fkey");
//         });

//         modelBuilder.Entity<Person>(entity =>
//         {
//             entity.HasKey(e => e.PersonId).HasName("persons_pkey");

//             entity.ToTable("persons");

//             entity.Property(e => e.PersonId).HasColumnName("person_id");
//             entity.Property(e => e.Email)
//                 .HasMaxLength(100)
//                 .HasColumnName("email");
//             entity.Property(e => e.FirstName)
//                 .HasMaxLength(100)
//                 .HasColumnName("first_name");
//             entity.Property(e => e.LastName)
//                 .HasMaxLength(100)
//                 .HasColumnName("last_name");
//             entity.Property(e => e.UserId).HasColumnName("user_id");

//             entity.HasOne(d => d.User).WithMany(p => p.People)
//                 .HasForeignKey(d => d.UserId)
//                 .OnDelete(DeleteBehavior.ClientSetNull)
//                 .HasConstraintName("persons_user_id_fkey");
//         });

//         modelBuilder.Entity<Transaction>(entity =>
//         {
//             entity.HasKey(e => e.Id).HasName("transactions_pkey");

//             entity.ToTable("transactions");

//             entity.Property(e => e.Id)
//                 .HasDefaultValueSql("uuid_generate_v4()")
//                 .HasColumnName("id");
//             entity.Property(e => e.AccountNumber).HasColumnName("account_number");
//             entity.Property(e => e.AmountMinorUnit).HasColumnName("amount_minor_unit");
//             entity.Property(e => e.CreatedAt)
//                 .HasDefaultValueSql("CURRENT_TIMESTAMP")
//                 .HasColumnType("timestamp without time zone")
//                 .HasColumnName("created_at");
//             entity.Property(e => e.DeletedAt)
//                 .HasColumnType("timestamp without time zone")
//                 .HasColumnName("deleted_at");

//             entity.HasOne(d => d.AccountNumberNavigation).WithMany(p => p.Transactions)
//                 .HasForeignKey(d => d.AccountNumber)
//                 .OnDelete(DeleteBehavior.ClientSetNull)
//                 .HasConstraintName("transactions_account_number_fkey");
//         });

//         modelBuilder.Entity<User>(entity =>
//         {
//             entity.HasKey(e => e.Id).HasName("users_pkey");

//             entity.ToTable("users");

//             entity.Property(e => e.Id)
//                 .HasDefaultValueSql("uuid_generate_v4()")
//                 .HasColumnName("id");
//             entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
//             entity.Property(e => e.Salt).HasColumnName("salt");
//             entity.Property(e => e.Username)
//                 .HasMaxLength(50)
//                 .HasColumnName("username");
//         });

//         OnModelCreatingPartial(modelBuilder);
//     }

//     partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
// }
