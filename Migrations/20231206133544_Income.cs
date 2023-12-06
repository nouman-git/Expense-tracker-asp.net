using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTrack.Migrations
{
    /// <inheritdoc />
    public partial class Income : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Income",
                table: "AspNetUsers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Income",
                table: "AspNetUsers");
        }
    }
}
