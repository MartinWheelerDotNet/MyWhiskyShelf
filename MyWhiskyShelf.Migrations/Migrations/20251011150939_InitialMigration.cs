using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace MyWhiskyShelf.Migrations.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:citext", ",,")
                .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,")
                .Annotation("Npgsql:PostgresExtension:vector", ",,");
            
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "citext", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Name",
                table: "Countries",
                column: "Name",
                unique: true);
            
            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "citext", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CountryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Regions_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            
            migrationBuilder.CreateIndex(
                name: "IX_Regions_CountryId_Name",
                table: "Regions",
                columns: new[] { "CountryId", "Name" },
                unique: true);

            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "citext", maxLength: 75, nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });
            
            migrationBuilder.CreateIndex(
                name: "UX_Brands_Name_eq",
                table: "Brands",
                column: "Name",
                unique: true);
            
            migrationBuilder.Sql(
                """
                CREATE INDEX IF NOT EXISTS "IX_Brands_Name_trgm"
                ON "Brands" USING gin ("Name" gin_trgm_ops);
                """);
            
            migrationBuilder.CreateTable(
                name: "Distilleries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "citext", maxLength: 100, nullable: false),
                    CountryId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Founded = table.Column<int>(type: "integer", nullable: false),
                    Owner = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    TastingNotes = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    FlavourVector = table.Column<Vector>(type: "vector(5)", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Distilleries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Distilleries_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Distilleries_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            
            migrationBuilder.CreateIndex(
                name: "UX_Distilleries_Name_eq",
                table: "Distilleries",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Distilleries_CountryId",
                table: "Distilleries",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Distilleries_RegionId",
                table: "Distilleries",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Distilleries_FlavourVector",
                table: "Distilleries",
                column: "FlavourVector")
                .Annotation("Npgsql:IndexMethod", "ivfflat")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" })
                .Annotation("Npgsql:StorageParameter:lists", 100);
            
            migrationBuilder.Sql(
                """
                CREATE INDEX IF NOT EXISTS "IX_Distilleries_Name_trgm"
                ON "Distilleries" USING gin ("Name" gin_trgm_ops);
                """);
            
            migrationBuilder.CreateTable(
                name: "WhiskyBottles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "citext", maxLength: 100, nullable: false),
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
                    FlavourVector = table.Column<Vector>(type: "vector(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhiskyBottles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WhiskyBottles_DistilleryName",
                table: "WhiskyBottles",
                column: "DistilleryName");

            migrationBuilder.CreateIndex(
                name: "IX_WhiskyBottles_Name_eq",
                table: "WhiskyBottles",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_WhiskyBottles_Status",
                table: "WhiskyBottles",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WhiskyBottles_FlavourVector",
                table: "WhiskyBottles",
                column: "FlavourVector")
                .Annotation("Npgsql:IndexMethod", "ivfflat")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" })
                .Annotation("Npgsql:StorageParameter:lists", 100);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "WhiskyBottles");
            migrationBuilder.DropTable(name: "Distilleries");
            migrationBuilder.DropTable(name: "Brands");
            migrationBuilder.DropTable(name: "Regions");
            migrationBuilder.DropTable(name: "Countries");
        }
    }
}
