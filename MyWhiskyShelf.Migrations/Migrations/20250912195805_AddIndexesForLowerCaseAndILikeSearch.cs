﻿using Microsoft.EntityFrameworkCore.Migrations;

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:pg_trgm", ",,");
        }
    }
}
