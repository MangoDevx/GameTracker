using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tracker.Migrations
{
    public partial class VersionOne : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WhitelistProcesses");

            migrationBuilder.RenameColumn(
                name: "ProcessName",
                table: "Processes",
                newName: "Path");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Processes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Tracking",
                table: "Processes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Blacklists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PathName = table.Column<string>(type: "TEXT", nullable: true),
                    FullPath = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blacklists", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Blacklists");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Processes");

            migrationBuilder.DropColumn(
                name: "Tracking",
                table: "Processes");

            migrationBuilder.RenameColumn(
                name: "Path",
                table: "Processes",
                newName: "ProcessName");

            migrationBuilder.CreateTable(
                name: "WhitelistProcesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProcessName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhitelistProcesses", x => x.Id);
                });
        }
    }
}
