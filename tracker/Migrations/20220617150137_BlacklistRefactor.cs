using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tracker.Migrations
{
    public partial class BlacklistRefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PathName",
                table: "Blacklists",
                newName: "Path");

            migrationBuilder.RenameColumn(
                name: "FullPath",
                table: "Blacklists",
                newName: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Path",
                table: "Blacklists",
                newName: "PathName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Blacklists",
                newName: "FullPath");
        }
    }
}
