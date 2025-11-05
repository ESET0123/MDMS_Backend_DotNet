using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MDMS_Backend.Migrations
{
    /// <inheritdoc />
    public partial class testingMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Manufacturers",
                columns: table => new
                {
                    ManufacturerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Manufact__357E5CA1D5BD4EDC", x => x.ManufacturerID);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Roles__8AFACE3A6C41C934", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "Statuses",
                columns: table => new
                {
                    StatusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Statuses__C8EE2043B2F0D9A7", x => x.StatusID);
                });

            migrationBuilder.CreateTable(
                name: "Tariffs",
                columns: table => new
                {
                    TariffID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    BaseRate = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Tariffs__EBAF9D931C1E783F", x => x.TariffID);
                });

            migrationBuilder.CreateTable(
                name: "Zones",
                columns: table => new
                {
                    ZoneID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ZoneName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Zones__601667953329FE7E", x => x.ZoneID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserNumber = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    PasswordHashed = table.Column<byte[]>(type: "varbinary(256)", maxLength: 256, nullable: false),
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserNumber);
                    table.ForeignKey(
                        name: "FK_Users_Roles",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "RoleID");
                });

            migrationBuilder.CreateTable(
                name: "Consumers",
                columns: table => new
                {
                    ConsumerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StatusID = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Consumer__63BBE99A874A2B45", x => x.ConsumerID);
                    table.ForeignKey(
                        name: "FK_Consumers_Statuses",
                        column: x => x.StatusID,
                        principalTable: "Statuses",
                        principalColumn: "StatusID");
                });

            migrationBuilder.CreateTable(
                name: "TariffSlabs",
                columns: table => new
                {
                    SlabID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TariffID = table.Column<int>(type: "int", nullable: false),
                    FromKWh = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ToKWh = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    RatePerKWh = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TariffSl__D61699010C08E430", x => x.SlabID);
                    table.ForeignKey(
                        name: "FK_TariffSlabs_Tariffs",
                        column: x => x.TariffID,
                        principalTable: "Tariffs",
                        principalColumn: "TariffID");
                });

            migrationBuilder.CreateTable(
                name: "Substations",
                columns: table => new
                {
                    SubstationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubstationName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ZoneID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Substati__BB479C6F4C9F78BD", x => x.SubstationID);
                    table.ForeignKey(
                        name: "FK_Substations_Zones",
                        column: x => x.ZoneID,
                        principalTable: "Zones",
                        principalColumn: "ZoneID");
                });

            migrationBuilder.CreateTable(
                name: "Feeders",
                columns: table => new
                {
                    FeederID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeederName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SubstationID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Feeders__9B20B0FCC20F2522", x => x.FeederID);
                    table.ForeignKey(
                        name: "FK_Feeders_Substations",
                        column: x => x.SubstationID,
                        principalTable: "Substations",
                        principalColumn: "SubstationID");
                });

            migrationBuilder.CreateTable(
                name: "DTRs",
                columns: table => new
                {
                    DTRID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DTRName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FeederID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DTRs__F865635FADB6DCC5", x => x.DTRID);
                    table.ForeignKey(
                        name: "FK_DTRs_Feeders",
                        column: x => x.FeederID,
                        principalTable: "Feeders",
                        principalColumn: "FeederID");
                });

            migrationBuilder.CreateTable(
                name: "Meters",
                columns: table => new
                {
                    MeterID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConsumerID = table.Column<int>(type: "int", nullable: false),
                    DTRID = table.Column<int>(type: "int", nullable: false),
                    IPAddress = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    ICCID = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IMSI = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ManufacturerID = table.Column<int>(type: "int", nullable: false),
                    Firmware = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    TariffID = table.Column<int>(type: "int", nullable: false),
                    InstallDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StatusID = table.Column<int>(type: "int", nullable: false),
                    LatestReading = table.Column<decimal>(type: "decimal(18,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Meters__59223B8C0448709E", x => x.MeterID);
                    table.ForeignKey(
                        name: "FK_Meters_Consumers",
                        column: x => x.ConsumerID,
                        principalTable: "Consumers",
                        principalColumn: "ConsumerID");
                    table.ForeignKey(
                        name: "FK_Meters_DTRs",
                        column: x => x.DTRID,
                        principalTable: "DTRs",
                        principalColumn: "DTRID");
                    table.ForeignKey(
                        name: "FK_Meters_Manufacturers",
                        column: x => x.ManufacturerID,
                        principalTable: "Manufacturers",
                        principalColumn: "ManufacturerID");
                    table.ForeignKey(
                        name: "FK_Meters_Statuses",
                        column: x => x.StatusID,
                        principalTable: "Statuses",
                        principalColumn: "StatusID");
                    table.ForeignKey(
                        name: "FK_Meters_Tariffs",
                        column: x => x.TariffID,
                        principalTable: "Tariffs",
                        principalColumn: "TariffID");
                });

            migrationBuilder.CreateTable(
                name: "MonthlyBills",
                columns: table => new
                {
                    BillID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MeterID = table.Column<int>(type: "int", nullable: false),
                    ConsumerID = table.Column<int>(type: "int", nullable: false),
                    BillingMonth = table.Column<DateOnly>(type: "date", nullable: false),
                    ConsumerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TariffName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MonthlyReadingKwh = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    BaseRate = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TotalBill = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BillStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MonthlyB__11F2FC4A0301E136", x => x.BillID);
                    table.ForeignKey(
                        name: "FK_MonthlyBills_Meters",
                        column: x => x.MeterID,
                        principalTable: "Meters",
                        principalColumn: "MeterID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Consumers_StatusID",
                table: "Consumers",
                column: "StatusID");

            migrationBuilder.CreateIndex(
                name: "IX_DTRs_FeederID",
                table: "DTRs",
                column: "FeederID");

            migrationBuilder.CreateIndex(
                name: "UQ__DTRs__263F444BB115D569",
                table: "DTRs",
                column: "DTRName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feeders_SubstationID",
                table: "Feeders",
                column: "SubstationID");

            migrationBuilder.CreateIndex(
                name: "UQ__Feeders__FB00FBD9938761C9",
                table: "Feeders",
                column: "FeederName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Manufact__737584F6F5BBDD81",
                table: "Manufacturers",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Meters_ConsumerID",
                table: "Meters",
                column: "ConsumerID");

            migrationBuilder.CreateIndex(
                name: "IX_Meters_DTRID",
                table: "Meters",
                column: "DTRID");

            migrationBuilder.CreateIndex(
                name: "IX_Meters_ManufacturerID",
                table: "Meters",
                column: "ManufacturerID");

            migrationBuilder.CreateIndex(
                name: "IX_Meters_StatusID",
                table: "Meters",
                column: "StatusID");

            migrationBuilder.CreateIndex(
                name: "IX_Meters_TariffID",
                table: "Meters",
                column: "TariffID");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyBills_MeterID",
                table: "MonthlyBills",
                column: "MeterID");

            migrationBuilder.CreateIndex(
                name: "UQ__Roles__8A2B61605D26D555",
                table: "Roles",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Roles__9C41170EA06E4C1A",
                table: "Roles",
                column: "Abbreviation",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Statuses__737584F6A0CBD582",
                table: "Statuses",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Substations_ZoneID",
                table: "Substations",
                column: "ZoneID");

            migrationBuilder.CreateIndex(
                name: "UQ__Substati__32F7515900B0DA0D",
                table: "Substations",
                column: "SubstationName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TariffSlabs_TariffID",
                table: "TariffSlabs",
                column: "TariffID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleID",
                table: "Users",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "UQ__Users__A9D105349FC0BA7D",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_UserID",
                table: "Users",
                column: "UserID",
                unique: true,
                filter: "[UserID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ__Zones__EE0DD16841D70E1E",
                table: "Zones",
                column: "ZoneName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonthlyBills");

            migrationBuilder.DropTable(
                name: "TariffSlabs");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Meters");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Consumers");

            migrationBuilder.DropTable(
                name: "DTRs");

            migrationBuilder.DropTable(
                name: "Manufacturers");

            migrationBuilder.DropTable(
                name: "Tariffs");

            migrationBuilder.DropTable(
                name: "Statuses");

            migrationBuilder.DropTable(
                name: "Feeders");

            migrationBuilder.DropTable(
                name: "Substations");

            migrationBuilder.DropTable(
                name: "Zones");
        }
    }
}
