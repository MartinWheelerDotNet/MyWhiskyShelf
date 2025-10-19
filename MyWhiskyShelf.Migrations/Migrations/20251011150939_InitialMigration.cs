using System;
using System.Runtime.InteropServices.Marshalling;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

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
                .Annotation("Npgsql:PostgresExtension:citext", ",,")
                .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,")
                .Annotation("Npgsql:PostgresExtension:vector", ",,");
            
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
                    Slug = table.Column<string>(type: "citext", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Distilleries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "citext", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Region = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
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
                });

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

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "citext", maxLength: 50, nullable: false),
                    Slug = table.Column<string>(type: "citext", maxLength: 50, nullable: false),
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
                name: "IX_Countries_Name",
                table: "Countries",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Slug",
                table: "Countries",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Distilleries_FlavourVector",
                table: "Distilleries",
                column: "FlavourVector")
                .Annotation("Npgsql:IndexMethod", "ivfflat")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" })
                .Annotation("Npgsql:StorageParameter:lists", 100);

            migrationBuilder.CreateIndex(
                name: "IX_Distilleries_Name_eq",
                table: "Distilleries",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Distilleries_Name_Id",
                table: "Distilleries",
                columns: new[] { "Name", "Id" },
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
                name: "IX_Regions_CountryId_Name",
                table: "Regions",
                columns: new[] { "CountryId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Regions_CountryId_Slug",
                table: "Regions",
                columns: new[] { "CountryId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WhiskyBottles_DistilleryName",
                table: "WhiskyBottles",
                column: "DistilleryName");

            migrationBuilder.CreateIndex(
                name: "IX_WhiskyBottles_FlavourVector",
                table: "WhiskyBottles",
                column: "FlavourVector")
                .Annotation("Npgsql:IndexMethod", "ivfflat")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" })
                .Annotation("Npgsql:StorageParameter:lists", 100);

            migrationBuilder.CreateIndex(
                name: "IX_WhiskyBottles_Name_eq",
                table: "WhiskyBottles",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_WhiskyBottles_Status",
                table: "WhiskyBottles",
                column: "Status");
            
            migrationBuilder.Sql(
                """
                CREATE INDEX IF NOT EXISTS IX_Distilleries_Name_trgm
                ON "Distilleries" USING gin ("Name" gin_trgm_ops);
                """);
            
            migrationBuilder.Sql(
                """
                CREATE INDEX IF NOT EXISTS IX_WhiskyBottles_Name_trgm
                ON "WhiskyBottles" USING gin ("Name" gin_trgm_ops);
                """); 
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Distilleries");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.DropTable(
                name: "WhiskyBottles");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
