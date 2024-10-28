﻿using KinopoiskDB.Core.Enum;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KinopoiskDB.Dal.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddEnumType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:month", "january,february,march,april,may,june,july,august,september,october,november,december");

            migrationBuilder.AddColumn<Month>(
                name: "month",
                table: "movies",
                type: "month",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "month",
                table: "movies");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:Enum:month", "january,february,march,april,may,june,july,august,september,october,november,december");
        }
    }
}
