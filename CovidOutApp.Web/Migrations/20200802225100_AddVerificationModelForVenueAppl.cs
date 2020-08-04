using Microsoft.EntityFrameworkCore.Migrations;

namespace CovidOutApp.Web.Migrations
{
    public partial class AddVerificationModelForVenueAppl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Verification",
                table: "VenueRegistrationApplications",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Verification",
                table: "VenueRegistrationApplications");
        }
    }
}
