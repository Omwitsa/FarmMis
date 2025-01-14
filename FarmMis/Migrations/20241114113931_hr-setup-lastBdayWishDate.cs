using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AAAErp.Migrations
{
    /// <inheritdoc />
    public partial class hrsetuplastBdayWishDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastBirthdayWishDate",
                table: "HrSetup",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NoOfStaffWished",
                table: "HrSetup",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastBirthdayWishDate",
                table: "HrSetup");

            migrationBuilder.DropColumn(
                name: "NoOfStaffWished",
                table: "HrSetup");
        }
    }
}
