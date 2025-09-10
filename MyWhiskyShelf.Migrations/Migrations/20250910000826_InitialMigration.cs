using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyWhiskyShelf.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,")
                .Annotation("Npgsql:PostgresExtension:unaccent", ",,");

            migrationBuilder.CreateTable(
                name: "Distilleries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Region = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Founded = table.Column<int>(type: "integer", nullable: false),
                    Owner = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    EncodedFlavourProfile = table.Column<long>(type: "bigint", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Distilleries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WhiskyBottles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DistilleryName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DistilleryId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<int>(type: "integer", maxLength: 15, nullable: false),
                    Bottler = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    YearBottled = table.Column<int>(type: "integer", nullable: true),
                    BatchNumber = table.Column<int>(type: "integer", nullable: true),
                    CaskNumber = table.Column<int>(type: "integer", nullable: true),
                    AbvPercentage = table.Column<decimal>(type: "numeric(4,1)", precision: 4, scale: 1, nullable: false),
                    VolumeCl = table.Column<int>(type: "integer", nullable: false),
                    VolumeRemainingCl = table.Column<int>(type: "integer", nullable: false),
                    AddedColouring = table.Column<bool>(type: "boolean", nullable: true),
                    ChillFiltered = table.Column<bool>(type: "boolean", nullable: true),
                    EncodedFlavourProfile = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhiskyBottles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Distilleries_Name",
                table: "Distilleries",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Distilleries_Owner",
                table: "Distilleries",
                column: "Owner");

            migrationBuilder.CreateIndex(
                name: "IX_Distilleries_Region",
                table: "Distilleries",
                column: "Region");

            migrationBuilder.CreateIndex(
                name: "IX_Distilleries_Type",
                table: "Distilleries",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_WhiskyBottles_DistilleryName",
                table: "WhiskyBottles",
                column: "DistilleryName");

            migrationBuilder.CreateIndex(
                name: "IX_WhiskyBottles_Status",
                table: "WhiskyBottles",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Distilleries");

            migrationBuilder.DropTable(
                name: "WhiskyBottles");
        }
    }
}
