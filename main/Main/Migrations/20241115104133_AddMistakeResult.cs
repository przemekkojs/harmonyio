using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Main.Migrations
{
    /// <inheritdoc />
    public partial class AddMistakeResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlgorithmOpinion",
                table: "ExcersiseResults");

            migrationBuilder.CreateTable(
                name: "MistakeResult",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExcersiseResultId = table.Column<int>(type: "INTEGER", nullable: false),
                    Bars = table.Column<string>(type: "TEXT", nullable: false),
                    Functions = table.Column<string>(type: "TEXT", nullable: false),
                    MistakeCodes = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MistakeResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MistakeResult_ExcersiseResults_ExcersiseResultId",
                        column: x => x.ExcersiseResultId,
                        principalTable: "ExcersiseResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MistakeResult_ExcersiseResultId",
                table: "MistakeResult",
                column: "ExcersiseResultId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MistakeResult");

            migrationBuilder.AddColumn<string>(
                name: "AlgorithmOpinion",
                table: "ExcersiseResults",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
