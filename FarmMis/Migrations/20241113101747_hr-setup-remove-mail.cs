using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AAAErp.Migrations
{
    /// <inheritdoc />
    public partial class hrsetupremovemail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HrMail",
                table: "HrSetup");

            migrationBuilder.DropColumn(
                name: "HrMailPwd",
                table: "HrSetup");

            migrationBuilder.AddColumn<string>(
                name: "HrMail",
                table: "SysSetup",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "HrMailPwd",
                table: "SysSetup",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HrMail",
                table: "SysSetup");

            migrationBuilder.DropColumn(
                name: "HrMailPwd",
                table: "SysSetup");

            migrationBuilder.AddColumn<string>(
                name: "HrMail",
                table: "HrSetup",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "HrMailPwd",
                table: "HrSetup",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
