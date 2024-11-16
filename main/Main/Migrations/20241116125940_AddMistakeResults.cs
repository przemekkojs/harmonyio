using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Main.Migrations
{
    /// <inheritdoc />
    public partial class AddMistakeResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MistakeResult_ExcersiseResults_ExcersiseResultId",
                table: "MistakeResult");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MistakeResult",
                table: "MistakeResult");

            migrationBuilder.RenameTable(
                name: "MistakeResult",
                newName: "MistakeResults");

            migrationBuilder.RenameIndex(
                name: "IX_MistakeResult_ExcersiseResultId",
                table: "MistakeResults",
                newName: "IX_MistakeResults_ExcersiseResultId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MistakeResults",
                table: "MistakeResults",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MistakeResults_ExcersiseResults_ExcersiseResultId",
                table: "MistakeResults",
                column: "ExcersiseResultId",
                principalTable: "ExcersiseResults",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MistakeResults_ExcersiseResults_ExcersiseResultId",
                table: "MistakeResults");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MistakeResults",
                table: "MistakeResults");

            migrationBuilder.RenameTable(
                name: "MistakeResults",
                newName: "MistakeResult");

            migrationBuilder.RenameIndex(
                name: "IX_MistakeResults_ExcersiseResultId",
                table: "MistakeResult",
                newName: "IX_MistakeResult_ExcersiseResultId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MistakeResult",
                table: "MistakeResult",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MistakeResult_ExcersiseResults_ExcersiseResultId",
                table: "MistakeResult",
                column: "ExcersiseResultId",
                principalTable: "ExcersiseResults",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
