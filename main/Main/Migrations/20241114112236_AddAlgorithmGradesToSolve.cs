using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Main.Migrations
{
    /// <inheritdoc />
    public partial class AddAlgorithmGradesToSolve : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AlgorithmOpinion",
                table: "ExcersiseResults",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "AlgorithmPoints",
                table: "ExcersiseResults",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlgorithmOpinion",
                table: "ExcersiseResults");

            migrationBuilder.DropColumn(
                name: "AlgorithmPoints",
                table: "ExcersiseResults");
        }
    }
}
