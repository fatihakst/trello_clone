using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrelloClone.API.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderToLists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Lists",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Lists");
        }
    }
}
