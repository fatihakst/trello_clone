using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrelloClone.API.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskLabels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LabelColor",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LabelText",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LabelColor",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "LabelText",
                table: "Tasks");
        }
    }
}
