using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CovidOutApp.Web.Migrations
{
    public partial class VenueRegistrationApplications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VenueRegistrationApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    VenueId = table.Column<Guid>(nullable: false),
                    AppliedById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateAppoved = table.Column<DateTime>(nullable: false),
                    ApprovedById = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VenueRegistrationApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VenueRegistrationApplications_AspNetUsers_AppliedById",
                        column: x => x.AppliedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VenueRegistrationApplications_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VenueRegistrationApplications_AppliedById",
                table: "VenueRegistrationApplications",
                column: "AppliedById");

            migrationBuilder.CreateIndex(
                name: "IX_VenueRegistrationApplications_ApprovedById",
                table: "VenueRegistrationApplications",
                column: "ApprovedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VenueRegistrationApplications");
        }
    }
}
