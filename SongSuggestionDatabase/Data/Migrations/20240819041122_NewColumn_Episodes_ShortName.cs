using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SongSuggestionDatabase.Data.Migrations
{
    /// <inheritdoc />
    public partial class NewColumn_Episodes_ShortName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                table: "Episodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "BannedList",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IgnoreInChecks = table.Column<bool>(type: "bit", nullable: false),
                    IsPermanentlyBanned = table.Column<bool>(type: "bit", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BannedList", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BannedList");

            migrationBuilder.DropColumn(
                name: "ShortName",
                table: "Episodes");
        }
    }
}
