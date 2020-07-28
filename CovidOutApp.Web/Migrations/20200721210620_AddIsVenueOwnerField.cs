using Microsoft.EntityFrameworkCore.Migrations;

namespace CovidOutApp.Web.Migrations
{
    public partial class AddIsVenueOwnerField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVenueOwner",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVenueOwner",
                table: "AspNetUsers");
        }
    }
}
