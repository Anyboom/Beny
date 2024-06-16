using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beny.Migrations
{
    /// <inheritdoc />
    public partial class AddTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FootballEventTag",
                columns: table => new
                {
                    FootballEventId = table.Column<int>(type: "INTEGER", nullable: false),
                    TagId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FootballEventTag", x => new { x.FootballEventId, x.TagId });
                    table.ForeignKey(
                        name: "FK_FootballEventTag_FootballEvents_FootballEventId",
                        column: x => x.FootballEventId,
                        principalTable: "FootballEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FootballEventTag_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sports_Name",
                table: "Sports",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Forecasts_Name",
                table: "Forecasts",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Competitions_Name",
                table: "Competitions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FootballEventTag_TagId",
                table: "FootballEventTag",
                column: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FootballEventTag");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Sports_Name",
                table: "Sports");

            migrationBuilder.DropIndex(
                name: "IX_Forecasts_Name",
                table: "Forecasts");

            migrationBuilder.DropIndex(
                name: "IX_Competitions_Name",
                table: "Competitions");
        }
    }
}
