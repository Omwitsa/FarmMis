using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AAAErp.Migrations
{
    /// <inheritdoc />
    public partial class hrsetupaddbirthdayimg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BirthdayImg",
                table: "HrSetup",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BirthdayImg",
                table: "HrSetup");
        }
    }
}
