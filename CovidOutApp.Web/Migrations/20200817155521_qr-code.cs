using Microsoft.EntityFrameworkCore.Migrations;

namespace CovidOutApp.Web.Migrations
{
    public partial class qrcode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QRCodeImageUrl",
                table: "Venues",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QRCodeImageUrl",
                table: "Venues");
        }
    }
}
