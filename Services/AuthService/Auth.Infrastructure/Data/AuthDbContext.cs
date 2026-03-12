using Auth.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Data;

public partial class AuthDbContext : DbContext
{
    public AuthDbContext()
    {
    }

    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Tenant> Tenants { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=ConnectionStrings:AuthDb");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("dblink");

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(255)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.TenantId).HasName("tenants_pkey");

            entity.ToTable("tenants");

            entity.HasIndex(e => e.SchemaName, "tenants_schema_name_key").IsUnique();

            entity.Property(e => e.TenantId)
                .HasDefaultValueSql("nextval('tenants_tenant_id_seq1'::regclass)")
                .HasColumnName("tenant_id");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(255)
                .HasColumnName("company_name");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_on");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.SchemaName)
                .HasMaxLength(63)
                .HasColumnName("schema_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.HasIndex(e => e.Username, "users_username_key").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.CreatedOn)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_on");
            entity.Property(e => e.DateOfBirth)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_of_birth");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.EmploymentStartDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("employment_start_date");
            entity.Property(e => e.FailedLoginCount)
                .HasDefaultValue(0)
                .HasColumnName("failed_login_count");
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .HasColumnName("first_name");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.IsTwoFactorEnabled)
                .HasDefaultValue(false)
                .HasColumnName("is_two_factor_enabled");
            entity.Property(e => e.LastFailedLogin)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_failed_login");
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .HasColumnName("last_name");
            entity.Property(e => e.LockoutUntil)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("lockout_until");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("phone");
            entity.Property(e => e.Position)
                .HasMaxLength(100)
                .HasColumnName("position");
            entity.Property(e => e.ProfilePicture)
                .HasColumnType("character varying")
                .HasColumnName("profile_picture");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.TwoFactorEnabledOn)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("two_factor_enabled_on");
            entity.Property(e => e.TwoFactorSecret).HasColumnName("two_factor_secret");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");
            entity.Property(e => e.Zipcode)
                .HasMaxLength(15)
                .HasColumnName("zipcode");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_role");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Users)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("fk_tenant");
        });
        modelBuilder.HasSequence("global_users_global_user_id_seq");
        modelBuilder.HasSequence("tenants_tenant_id_seq");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
