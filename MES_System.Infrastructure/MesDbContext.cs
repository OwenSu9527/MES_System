using System;
using System.Collections.Generic;
using MES_System.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MES_System.Infrastructure;

public partial class MesDbContext : DbContext
{
    public MesDbContext()
    {
    }

    public MesDbContext(DbContextOptions<MesDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Equipment> Equipments { get; set; }

    public virtual DbSet<Station> Stations { get; set; }

    public virtual DbSet<WipSnapshot> WipSnapshots { get; set; }

    public virtual DbSet<WorkOrder> WorkOrders { get; set; }

//      連線字串已經由 Program.cs 統一管理
//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=localhost;Database=MesDb;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.Property(e => e.LastUpdated).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Station>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<WipSnapshot>(entity =>
        {
            entity.Property(e => e.LastUpdated).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.StationName).HasMaxLength(100);
            entity.Property(e => e.WorkOrderNo).HasMaxLength(50);
        });

        modelBuilder.Entity<WorkOrder>(entity =>
        {
            entity.Property(e => e.OrderNo).HasMaxLength(50);
            entity.Property(e => e.Product).HasMaxLength(100);
            entity.Property(e => e.StartTime).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("New");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
