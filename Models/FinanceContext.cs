// using System;
// using System.Collections.Generic;
// using Microsoft.EntityFrameworkCore;

// namespace FinanceApp.Models;

// public partial class FinanceContext : DbContext
// {
//     public FinanceContext()
//     {
//     }

//     public FinanceContext(DbContextOptions<FinanceContext> options)
//         : base(options)
//     {
//     }

//     public virtual DbSet<User> Users { get; set; }

//     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
// #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//         => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=password;Database=finance");

//     protected override void OnModelCreating(ModelBuilder modelBuilder)
//     {
//         modelBuilder.Entity<User>(entity =>
//         {
//             entity.HasKey(e => e.Id).HasName("users_pkey");

//             entity.ToTable("users");

//             entity.Property(e => e.Id).HasColumnName("id");
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
