using System;
using System.Collections.Generic;
using Employee.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Employee.Infrastructure.Data;

public partial class TenantDbContext : DbContext
{
    public TenantDbContext()
    {
    }

    public TenantDbContext(DbContextOptions<TenantDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Department> Departments { get; set; }
    public virtual DbSet<EmployeeList> EmployeeLists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("dblink");

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("departments_pkey");

            entity.ToTable("departments");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('department_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.ManagerId).HasColumnName("manager_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<EmployeeList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employees_pkey");

            entity.ToTable("employee_list");

            entity.HasIndex(e => e.Email, "employees_email_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('employees_id_seq'::regclass)")
                .HasColumnName("id");

            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_on");

            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Salary).HasColumnName("salary");

            entity.HasOne(d => d.Department).WithMany(p => p.EmployeeLists)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("employees_department_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}