using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SongSuggestionDatabase.Data.Migrations
{
    /// <inheritdoc />
    public partial class NewColumn_Songs_Status : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Songs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Songs");
        }
    }
}
