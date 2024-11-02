using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KinopoiskDB.Dal.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class updColumnPremiereRu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "premiereRu",
                table: "movies",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "premiereRu",
                table: "movies",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }
    }
}
