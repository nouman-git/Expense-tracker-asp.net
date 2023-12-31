using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTrack.Migrations
{
    /// <inheritdoc />
    public partial class CreditDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CreditDate",
                table: "UserInfo",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CreditDate",
                table: "AspNetUsers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreditDate",
                table: "UserInfo");

            migrationBuilder.DropColumn(
                name: "CreditDate",
                table: "AspNetUsers");
        }
    }
}
