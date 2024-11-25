using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Main.Migrations
{
    /// <inheritdoc />
    public partial class MoveShowAlgorithmOpinionToQuiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowAlgorithmOpinion",
                table: "QuizResults");

            migrationBuilder.AddColumn<bool>(
                name: "ShowAlgorithmOpinion",
                table: "Quizes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowAlgorithmOpinion",
                table: "Quizes");

            migrationBuilder.AddColumn<bool>(
                name: "ShowAlgorithmOpinion",
                table: "QuizResults",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
