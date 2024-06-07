using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QwiikAgency.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AGENCY",
                columns: table => new
                {
                    AGENCY_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AGENCY_NAME = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    AGENCY_ADDRESS = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    AGENCY_PHONE = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    AGENCY_MAX_APPOINTMENT = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agency", x => x.AGENCY_ID);
                });

            migrationBuilder.CreateTable(
                name: "APPOINTMENT",
                columns: table => new
                {
                    APPOINTMENT_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AGENCY_ID = table.Column<int>(type: "int", nullable: false),
                    CUST_ID = table.Column<int>(type: "int", nullable: false),
                    APPOINTMENT_DATE = table.Column<DateTime>(type: "smalldatetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointment", x => x.APPOINTMENT_ID);
                });

            migrationBuilder.CreateTable(
                name: "CUSTOMER",
                columns: table => new
                {
                    CUST_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CUST_NAME = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    CUST_ADDRESS = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    CUST_PHONE = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CUSTOMER", x => x.CUST_ID);
                });

            migrationBuilder.CreateTable(
                name: "OFFDAY",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    AGENCY_ID = table.Column<int>(type: "int", nullable: false),
                    OFF_DATE = table.Column<DateTime>(type: "smalldatetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OFFDAY", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AGENCY");

            migrationBuilder.DropTable(
                name: "APPOINTMENT");

            migrationBuilder.DropTable(
                name: "CUSTOMER");

            migrationBuilder.DropTable(
                name: "OFFDAY");
        }
    }
}
