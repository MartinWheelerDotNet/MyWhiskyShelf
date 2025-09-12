using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyWhiskyShelf.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexesForLowerCaseAndILikeSearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,");
            
            migrationBuilder.Sql(@"CREATE EXTENSION IF NOT EXISTS pg_trgm;");
            
            migrationBuilder.Sql(
                """
                CREATE INDEX IF NOT EXISTS ux_distilleries_name_lower
                ON "Distilleries" (lower("Name"));
                """);

            migrationBuilder.Sql(
                """
                CREATE INDEX IF NOT EXISTS ix_distilleries_name_trgm
                ON "Distilleries" USING gin (lower("Name") gin_trgm_ops);
                """);
            
            migrationBuilder.Sql(
                """
                CREATE INDEX IF NOT EXISTS ux_whisky_bottles_name_lower
                ON "WhiskyBottles" (lower("Name"));
                """);
            
            migrationBuilder.Sql(
                """
                CREATE INDEX IF NOT EXISTS ix_whisky_bottles_name_trgm
                ON "WhiskyBottles" USING gin (lower("Name") gin_trgm_ops);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:pg_trgm", ",,");
        }
    }
}
