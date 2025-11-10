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

    public virtual DbSet<DailyMeterReading> DailyMeterReadings { get; set; }

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

    public virtual DbSet<TodRule> TodRules { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Zone> Zones { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:mdms-sql-server.database.windows.net,1433;Initial Catalog=MDMS_DB;Persist Security Info=False;User ID=sqladmin;Password=manjit@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Timeout=30");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Consumer>(entity =>
        {
            entity.HasKey(e => e.ConsumerId).HasName("PK__Consumer__63BBE99A38833ADC");

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

        modelBuilder.Entity<DailyMeterReading>(entity =>
        {
            entity.HasKey(e => e.ReadingId).HasName("PK__DailyMet__C80F9C4EBE241525");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.BaseRate).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.ConsumptionKwh).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CurrentReading).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.DiscountPercent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.EffectiveRate).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.PreviousReading).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.RecordedBy).HasMaxLength(100);
            entity.Property(e => e.SurgeChargePercent).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Meter).WithMany(p => p.DailyMeterReadings)
                .HasForeignKey(d => d.MeterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DailyMete__Meter__56E8E7AB");

            entity.HasOne(d => d.TodRule).WithMany(p => p.DailyMeterReadings)
                .HasForeignKey(d => d.TodRuleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DailyMete__TodRu__57DD0BE4");
        });

        modelBuilder.Entity<Dtr>(entity =>
        {
            entity.HasKey(e => e.Dtrid).HasName("PK__DTRs__F865635F0833BB90");

            entity.ToTable("DTRs");

            entity.HasIndex(e => e.Dtrname, "UQ__DTRs__263F444B31094446").IsUnique();

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
            entity.HasKey(e => e.FeederId).HasName("PK__Feeders__9B20B0FCF29B5EC1");

            entity.HasIndex(e => e.FeederName, "UQ__Feeders__FB00FBD996BC6714").IsUnique();

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
            entity.HasKey(e => e.ManufacturerId).HasName("PK__Manufact__357E5CA1C373694F");

            entity.HasIndex(e => e.Name, "UQ__Manufact__737584F60A81B2BD").IsUnique();

            entity.Property(e => e.ManufacturerId).HasColumnName("ManufacturerID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Meter>(entity =>
        {
            entity.HasKey(e => e.MeterId).HasName("PK__Meters__59223B8CEA53528E");

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
            entity.HasKey(e => e.BillId).HasName("PK__MonthlyB__11F2FC6A2817D668");

            entity.Property(e => e.BaseAmount).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.BillStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.GeneratedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.GeneratedBy).HasMaxLength(100);
            entity.Property(e => e.NetAmount).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.PaidDate).HasColumnType("datetime");
            entity.Property(e => e.TaxAmount).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TotalConsumptionKwh).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TotalDiscounts).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TotalSurgeCharges).HasColumnType("decimal(18, 4)");

            entity.HasOne(d => d.Consumer).WithMany(p => p.MonthlyBills)
                .HasForeignKey(d => d.ConsumerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MonthlyBi__Consu__607251E5");

            entity.HasOne(d => d.Meter).WithMany(p => p.MonthlyBills)
                .HasForeignKey(d => d.MeterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MonthlyBi__Meter__5F7E2DAC");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A48671FF2");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B616065804CAF").IsUnique();

            entity.HasIndex(e => e.Abbreviation, "UQ__Roles__9C41170E8B3F7400").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Abbreviation).HasMaxLength(3);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Statuses__C8EE2043C9A4B36E");

            entity.HasIndex(e => e.Name, "UQ__Statuses__737584F64E35F77A").IsUnique();

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.Name).HasMaxLength(20);
        });

        modelBuilder.Entity<Substation>(entity =>
        {
            entity.HasKey(e => e.SubstationId).HasName("PK__Substati__BB479C6F57ACF55C");

            entity.HasIndex(e => e.SubstationName, "UQ__Substati__32F751591DBB4E7E").IsUnique();

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
            entity.HasKey(e => e.TariffId).HasName("PK__Tariffs__EBAF9D932131DEA3");

            entity.Property(e => e.TariffId).HasColumnName("TariffID");
            entity.Property(e => e.BaseRate).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.TaxRate).HasColumnType("decimal(5, 2)");
        });

        modelBuilder.Entity<TariffSlab>(entity =>
        {
            entity.HasKey(e => e.SlabId).HasName("PK__TariffSl__D6169901A3122B22");

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

        modelBuilder.Entity<TodRule>(entity =>
        {
            entity.HasKey(e => e.TodRuleId).HasName("PK__TodRules__5A5E32F74245F87B");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.DiscountPercent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RuleName).HasMaxLength(100);
            entity.Property(e => e.SurgeChargePercent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.HasOne(d => d.Tariff).WithMany(p => p.TodRules)
                .HasForeignKey(d => d.TariffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TodRules__Tariff__531856C7");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserNumber);

            entity.ToTable(tb => tb.HasTrigger("trg_GenerateUserIDs"));

            entity.HasIndex(e => e.UserId, "UQ_UserID").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534B234CE11").IsUnique();

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
            entity.HasKey(e => e.ZoneId).HasName("PK__Zones__601667958DA976F4");

            entity.HasIndex(e => e.ZoneName, "UQ__Zones__EE0DD1685F78757F").IsUnique();

            entity.Property(e => e.ZoneId).HasColumnName("ZoneID");
            entity.Property(e => e.ZoneName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
