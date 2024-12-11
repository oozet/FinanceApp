using FinanceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // public AppDbContext(DbContextOptions<AppDbContext> options)
    //     : base(options) { }

    public DbSet<AppUser> Users { get; set; } = null!;

    //public DbSet<TransactionData> TransactionData { get; set; } = null!;


    // Set up account number sequence. 10 digit numbers starting with 9.
    public void EnsureDatabaseSetup()
    {
        this.Database.ExecuteSqlRaw(
            @"
            -- Create the sequence if it doesn't already exist
            DO $$
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM pg_sequences WHERE schemaname = 'public' AND sequencename = 'account_number_seq') THEN
                    CREATE SEQUENCE account_number_seq START 100000000 INCREMENT BY 1;
                END IF;
            END
            $$;

            -- Function to generate account numbers
            CREATE OR REPLACE FUNCTION generate_account_number() RETURNS trigger AS $$
            BEGIN
                NEW.account_number := '9' || LPAD(NEXTVAL('account_number_seq')::text, 9, '0');
                RETURN NEW;
            END;
            $$ LANGUAGE plpgsql;

            -- Create the trigger if it doesn't already exist
            DO $$
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM pg_trigger WHERE tgname = 'set_account_number') THEN
                    CREATE TRIGGER set_account_number
                    BEFORE INSERT ON accounts
                    FOR EACH ROW
                    EXECUTE FUNCTION generate_account_number();
                END IF;
            END
            $$;
        "
        );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Set up Enums and Uuid generation.
        modelBuilder
            .HasPostgresEnum("account_type", new[] { "Private", "Savings", "Business" })
            .HasPostgresEnum("transaction_type", new[] { "Deposit", "Withdrawal" })
            .HasPostgresExtension("uuid-ossp");

        // Set up tables.
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Username).HasMaxLength(50).HasColumnName("username");

            // Shadow properties.
            entity.Property<string>("PasswordHash").HasColumnName("password_hash").IsRequired();
            entity.Property<string>("Salt").HasColumnName("salt").IsRequired();
        });

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountNumber).HasName("accounts_pkey");

            entity.ToTable("accounts");

            entity
                .Property(e => e.AccountNumber)
                .HasMaxLength(10)
                .IsRequired()
                .HasColumnName("account_number");
            entity
                .Property(e => e.BalanceMinorUnit)
                .HasDefaultValue(0L)
                .HasColumnName("balance_minor_unit");
            entity
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity
                .Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity
                .HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("accounts_user_id_fkey");
        });

        modelBuilder.Entity<TransactionData>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("transactions_pkey");

            entity.ToTable("transactions");

            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()").HasColumnName("id");
            entity.Property(e => e.AccountNumber).HasColumnName("account_number");
            entity.Property(e => e.AmountMinorUnit).HasColumnName("amount_minor_unit");
            entity
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity
                .Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");

            entity
                .HasOne<Account>()
                .WithMany()
                .HasForeignKey(d => d.AccountNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("transactions_account_number_fkey");
        });
    }
}
