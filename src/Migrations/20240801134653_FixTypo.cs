using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Legiosoft_test_case.Migrations
{
    /// <inheritdoc />
    public partial class FixTypo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {                
			migrationBuilder.RenameColumn(
            name: "Ammount",
            table: "Transactions",
            newName: "Amount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.RenameColumn(
			name: "Amount",
			table: "Transactions",
			newName: "Ammount");
		}
    }
}
