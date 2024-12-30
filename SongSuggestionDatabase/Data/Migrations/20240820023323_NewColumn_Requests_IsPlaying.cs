using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SongSuggestionDatabase.Data.Migrations
{
    /// <inheritdoc />
    public partial class NewColumn_Requests_IsPlaying : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPlaying",
                table: "Songs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPlaying",
                table: "Songs");
        }
    }
}
