using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MDMS_Backend.Models;

public partial class MdmsDbContext : DbContext
{
    public MdmsDbContext()
    {
    }

    public MdmsDbContext(DbContextOptions<MdmsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Consumer> Consumers { get; set; }

    public virtual DbSet<Dtr> Dtrs { get; set; }

    public virtual DbSet<Feeder> Feeders { get; set; }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<Meter> Meters { get; set; }

    public virtual DbSet<MonthlyBill> MonthlyBills { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Substation> Substations { get; set; }

    public virtual DbSet<Tariff> Tariffs { get; set; }

    public virtual DbSet<TariffSlab> TariffSlabs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Zone> Zones { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-UDK5CRLP;Initial Catalog=MDMS_DB;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Consumer>(entity =>
        {
            entity.HasKey(e => e.ConsumerId).HasName("PK__Consumer__63BBE99A762B88C8");

            entity.Property(e => e.ConsumerId).HasColumnName("ConsumerID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);

            entity.HasOne(d => d.Status).WithMany(p => p.Consumers)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Consumers_Statuses");
        });

        modelBuilder.Entity<Dtr>(entity =>
        {
            entity.HasKey(e => e.Dtrid).HasName("PK__DTRs__F865635F75EF4A5D");

            entity.ToTable("DTRs");

            entity.HasIndex(e => e.Dtrname, "UQ__DTRs__263F444B01900264").IsUnique();

            entity.Property(e => e.Dtrid).HasColumnName("DTRID");
            entity.Property(e => e.Dtrname)
                .HasMaxLength(50)
                .HasColumnName("DTRName");
            entity.Property(e => e.FeederId).HasColumnName("FeederID");

            entity.HasOne(d => d.Feeder).WithMany(p => p.Dtrs)
                .HasForeignKey(d => d.FeederId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DTRs_Feeders");
        });

        modelBuilder.Entity<Feeder>(entity =>
        {
            entity.HasKey(e => e.FeederId).HasName("PK__Feeders__9B20B0FCE65F216E");

            entity.HasIndex(e => e.FeederName, "UQ__Feeders__FB00FBD93EB196D1").IsUnique();

            entity.Property(e => e.FeederId).HasColumnName("FeederID");
            entity.Property(e => e.FeederName).HasMaxLength(50);
            entity.Property(e => e.SubstationId).HasColumnName("SubstationID");

            entity.HasOne(d => d.Substation).WithMany(p => p.Feeders)
                .HasForeignKey(d => d.SubstationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Feeders_Substations");
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.ManufacturerId).HasName("PK__Manufact__357E5CA1B0DA0FA9");

            entity.HasIndex(e => e.Name, "UQ__Manufact__737584F6B56837BC").IsUnique();

            entity.Property(e => e.ManufacturerId).HasColumnName("ManufacturerID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Meter>(entity =>
        {
            entity.HasKey(e => e.MeterId).HasName("PK__Meters__59223B8CBD940B89");

            entity.Property(e => e.MeterId).HasColumnName("MeterID");
            entity.Property(e => e.ConsumerId).HasColumnName("ConsumerID");
            entity.Property(e => e.Dtrid).HasColumnName("DTRID");
            entity.Property(e => e.Firmware).HasMaxLength(10);
            entity.Property(e => e.Iccid)
                .HasMaxLength(20)
                .HasColumnName("ICCID");
            entity.Property(e => e.Imsi)
                .HasMaxLength(20)
                .HasColumnName("IMSI");
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(15)
                .HasColumnName("IPAddress");
            entity.Property(e => e.LatestReading).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.ManufacturerId).HasColumnName("ManufacturerID");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.TariffId).HasColumnName("TariffID");

            entity.HasOne(d => d.Consumer).WithMany(p => p.Meters)
                .HasForeignKey(d => d.ConsumerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Meters_Consumers");

            entity.HasOne(d => d.Dtr).WithMany(p => p.Meters)
                .HasForeignKey(d => d.Dtrid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Meters_DTRs");

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.Meters)
                .HasForeignKey(d => d.ManufacturerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Meters_Manufacturers");

            entity.HasOne(d => d.Status).WithMany(p => p.Meters)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Meters_Statuses");

            entity.HasOne(d => d.Tariff).WithMany(p => p.Meters)
                .HasForeignKey(d => d.TariffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Meters_Tariffs");
        });

        modelBuilder.Entity<MonthlyBill>(entity =>
        {
            entity.HasKey(e => e.BillId).HasName("PK__MonthlyB__11F2FC4ACC09B9EF");

            entity.Property(e => e.BillId).HasColumnName("BillID");
            entity.Property(e => e.BaseRate).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.BillStatus).HasMaxLength(20);
            entity.Property(e => e.ConsumerId).HasColumnName("ConsumerID");
            entity.Property(e => e.ConsumerName).HasMaxLength(100);
            entity.Property(e => e.MeterId).HasColumnName("MeterID");
            entity.Property(e => e.MonthlyReadingKwh).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TariffName).HasMaxLength(50);
            entity.Property(e => e.TaxRate).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TotalBill).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Meter).WithMany(p => p.MonthlyBills)
                .HasForeignKey(d => d.MeterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MonthlyBills_Meters");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A43C50701");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B616034548838").IsUnique();

            entity.HasIndex(e => e.Abbreviation, "UQ__Roles__9C41170EA863B898").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Abbreviation).HasMaxLength(3);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Statuses__C8EE2043E1AF1675");

            entity.HasIndex(e => e.Name, "UQ__Statuses__737584F6AA9C1DF2").IsUnique();

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.Name).HasMaxLength(20);
        });

        modelBuilder.Entity<Substation>(entity =>
        {
            entity.HasKey(e => e.SubstationId).HasName("PK__Substati__BB479C6F64F7EBA4");

            entity.HasIndex(e => e.SubstationName, "UQ__Substati__32F75159A3651B8C").IsUnique();

            entity.Property(e => e.SubstationId).HasColumnName("SubstationID");
            entity.Property(e => e.SubstationName).HasMaxLength(50);
            entity.Property(e => e.ZoneId).HasColumnName("ZoneID");

            entity.HasOne(d => d.Zone).WithMany(p => p.Substations)
                .HasForeignKey(d => d.ZoneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Substations_Zones");
        });

        modelBuilder.Entity<Tariff>(entity =>
        {
            entity.HasKey(e => e.TariffId).HasName("PK__Tariffs__EBAF9D9343A90C25");

            entity.Property(e => e.TariffId).HasColumnName("TariffID");
            entity.Property(e => e.BaseRate).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.TaxRate).HasColumnType("decimal(5, 2)");
        });

        modelBuilder.Entity<TariffSlab>(entity =>
        {
            entity.HasKey(e => e.SlabId).HasName("PK__TariffSl__D61699013F89B20C");

            entity.Property(e => e.SlabId).HasColumnName("SlabID");
            entity.Property(e => e.FromKwh)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("FromKWh");
            entity.Property(e => e.RatePerKwh)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("RatePerKWh");
            entity.Property(e => e.TariffId).HasColumnName("TariffID");
            entity.Property(e => e.ToKwh)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("ToKWh");

            entity.HasOne(d => d.Tariff).WithMany(p => p.TariffSlabs)
                .HasForeignKey(d => d.TariffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TariffSlabs_Tariffs");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserNumber);

            entity.ToTable(tb =>
                {
                    tb.HasTrigger("TR_GenerateUserID");
                    tb.HasTrigger("trg_GenerateUserIDs");
                });

            entity.HasIndex(e => e.UserId, "UQ_UserID").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053481335CA5").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.LastLogin).HasColumnType("datetime");
            entity.Property(e => e.PasswordHashed).HasMaxLength(256);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UserId)
                .HasMaxLength(10)
                .HasColumnName("UserID");
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        modelBuilder.Entity<Zone>(entity =>
        {
            entity.HasKey(e => e.ZoneId).HasName("PK__Zones__60166795AFBF2AF3");

            entity.HasIndex(e => e.ZoneName, "UQ__Zones__EE0DD168665A13E7").IsUnique();

            entity.Property(e => e.ZoneId).HasColumnName("ZoneID");
            entity.Property(e => e.ZoneName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
