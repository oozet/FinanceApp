using FinanceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // public AppDbContext(DbContextOptions<AppDbContext> options)
    //     : base(options) { }

    public DbSet<AppUser> Users { get; set; } = null!;
    public DbSet<Account> Accounts { get; set; } = null!;

    //public DbSet<TransactionData> TransactionData { get; set; } = null!;


    // Set up account number sequence. 10 digit numbers starting with 9.
    public void EnsureDatabaseSetup()
    {
        Database.ExecuteSqlRaw(
            @"
            DO $$
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM pg_sequences WHERE schemaname = 'public' AND sequencename = 'account_number_seq') THEN
                    CREATE SEQUENCE account_number_seq START 1 INCREMENT BY 1;
                ELSE
                    ALTER SEQUENCE account_number_seq RESTART WITH 1;
                END IF;
            END
            $$;

            -- Function to generate account numbers
            CREATE OR REPLACE FUNCTION generate_account_number() RETURNS trigger AS $$
            BEGIN
                IF NEW.account_type = 'personal' THEN
                    NEW.account_number := '9' || LPAD(NEXTVAL('account_number_seq')::text, 8, '0');
                ELSIF NEW.account_type = 'savings' THEN
                    NEW.account_number := '8' || LPAD(NEXTVAL('account_number_seq')::text, 8, '0');
                ELSIF NEW.account_type = 'business' THEN
                    NEW.account_number := '7' || LPAD(NEXTVAL('account_number_seq')::text, 8, '0');
                ELSE
                    NEW.account_number := '9' || LPAD(NEXTVAL('account_number_seq')::text, 8, '0');
                END IF;
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
            .HasPostgresEnum("account_type", new[] { "personal", "savings", "business" })
            .HasPostgresEnum("transaction_type", new[] { "deposit", "withdrawal" })
            .HasPostgresExtension("uuid-ossp");

        // modelBuilder
        //     .HasPostgresEnum<AccountType>("account_type")
        //     .HasPostgresEnum<TransactionType>("transaction_type")
        //     .HasPostgresExtension("uuid-ossp");

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
                .HasColumnName("account_number")
                .HasMaxLength(10)
                .ValueGeneratedNever()
                .HasColumnType("bigint");

            entity
                .Property(e => e.BalanceMinorUnit)
                .HasDefaultValue(0L)
                .HasColumnName("balance_minor_unit");
            entity
                .Property(e => e.AccountType)
                .HasColumnName("account_type")
                .HasConversion(
                    v => v.ToString(),
                    v => (AccountType)Enum.Parse(typeof(AccountType), v, true)
                )
                .HasColumnType("account_type");
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
                .Property(e => e.TransactionType)
                .HasColumnName("transaction_type")
                .HasConversion(
                    v => v.ToString(),
                    v => (TransactionType)Enum.Parse(typeof(TransactionType), v, true)
                )
                .HasColumnType("account_type");
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
