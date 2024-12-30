using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SongSuggestionDatabase.Data.Migrations
{
    /// <inheritdoc />
    public partial class NewControllers_EpisodesRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Episodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Format = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CatalogChecksEnabled = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BannedListChecksEnabled = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Episodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Songs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeRequested = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    USDAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Artist = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EpisodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RatingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DetailedRating = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quote = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Songs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Songs_Currencies_CurrencyCode",
                        column: x => x.CurrencyCode,
                        principalTable: "Currencies",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_Songs_Episodes_EpisodeId",
                        column: x => x.EpisodeId,
                        principalTable: "Episodes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Songs_Ratings_RatingId",
                        column: x => x.RatingId,
                        principalTable: "Ratings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Songs_CurrencyCode",
                table: "Songs",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_Songs_EpisodeId",
                table: "Songs",
                column: "EpisodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Songs_RatingId",
                table: "Songs",
                column: "RatingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Songs");

            migrationBuilder.DropTable(
                name: "Episodes");
        }
    }
}
