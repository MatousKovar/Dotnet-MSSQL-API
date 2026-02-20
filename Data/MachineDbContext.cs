using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SimpleAPI.Models;

namespace SimpleAPI.Data;

public partial class MachineDbContext : DbContext
{
    public MachineDbContext()
    {
    }

    public MachineDbContext(DbContextOptions<MachineDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Machine> Machines { get; set; }

    public virtual DbSet<MachineType> MachineTypes { get; set; }

    public virtual DbSet<Operator> Operators { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<WorkLog> WorkLogs { get; set; }

    public virtual DbSet<WorkType> WorkTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Machine>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__machines__3213E83F757E767D");

            entity.ToTable("machines");

            entity.HasIndex(e => e.Code, "UQ__machines__357D4CF9A758BABD").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .HasColumnName("code");
            entity.Property(e => e.Location)
                .HasMaxLength(50)
                .HasColumnName("location");
            entity.Property(e => e.MachineTypeId).HasColumnName("machine_type_id");
            entity.Property(e => e.PurchaseDate).HasColumnName("purchase_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");

            entity.HasOne(d => d.MachineType).WithMany(p => p.Machines)
                .HasForeignKey(d => d.MachineTypeId)
                .HasConstraintName("FK_Machines_Types");
        });

        modelBuilder.Entity<MachineType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__machine___3213E83F8EAB2780");

            entity.ToTable("machine_types");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.MaintenanceIntervalHours).HasColumnName("maintenance_interval_hours");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Operator>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__operator__3213E83FA4A2E945");

            entity.ToTable("operators");

            entity.HasIndex(e => e.BadgeNumber, "UQ__operator__3E4D103E8F2D35D0").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BadgeNumber)
                .HasMaxLength(20)
                .HasColumnName("badge_number");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__projects__3213E83FCB233B87");

            entity.ToTable("projects");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClientName)
                .HasMaxLength(100)
                .HasColumnName("client_name");
            entity.Property(e => e.Deadline).HasColumnName("deadline");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
        });

        modelBuilder.Entity<WorkLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__work_log__3213E83FD68994B7");

            entity.ToTable("work_logs");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.MachineId).HasColumnName("machine_id");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.OperatorId).HasColumnName("operator_id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
            entity.Property(e => e.WorkTypeId).HasColumnName("work_type_id");

            entity.HasOne(d => d.Machine).WithMany(p => p.WorkLogs)
                .HasForeignKey(d => d.MachineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Logs_Machines");

            entity.HasOne(d => d.Operator).WithMany(p => p.WorkLogs)
                .HasForeignKey(d => d.OperatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Logs_Operators");

            entity.HasOne(d => d.Project).WithMany(p => p.WorkLogs)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Logs_Projects");

            entity.HasOne(d => d.WorkType).WithMany(p => p.WorkLogs)
                .HasForeignKey(d => d.WorkTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Logs_Worktypes");
        });

        modelBuilder.Entity<WorkType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__work_typ__3213E83FAD7C3C63");

            entity.ToTable("work_types");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.WorkName).HasColumnName("work_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
