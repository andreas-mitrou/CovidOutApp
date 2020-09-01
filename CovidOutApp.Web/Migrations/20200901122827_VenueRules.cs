using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CovidOutApp.Web.Migrations
{
    public partial class VenueRules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VenueRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AffectedDateFrom = table.Column<DateTime>(nullable: false),
                    AffectedDateTo = table.Column<DateTime>(nullable: false),
                    MaxStayInHours = table.Column<double>(nullable: false),
                    Capacity = table.Column<DateTime>(nullable: false),
                    VenueId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VenueRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VenueRules_Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VenueRules_VenueId",
                table: "VenueRules",
                column: "VenueId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VenueRules");
        }
    }
}
