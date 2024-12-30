using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SongSuggestionDatabase.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceColumns_CatalogSortOptions_SortByParamValueOrderParamValue_Value : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderParamValue",
                table: "CatalogSortOptions");

            migrationBuilder.RenameColumn(
                name: "SortByParamValue",
                table: "CatalogSortOptions",
                newName: "Value");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "CatalogSortOptions",
                newName: "SortByParamValue");

            migrationBuilder.AddColumn<string>(
                name: "OrderParamValue",
                table: "CatalogSortOptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
