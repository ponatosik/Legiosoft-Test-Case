using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Legiosoft_test_case.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeZoneAndLocalTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LocalTime",
                table: "Transactions",
				type: "TEXT",
				nullable: false,
				defaultValue:"");

            migrationBuilder.AddColumn<string>(
                name: "IanaTimeZoneId",
                table: "Transactions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocalTime",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "IanaTimeZoneId",
                table: "Transactions");
        }
    }
}
