using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Main.Migrations
{
    /// <inheritdoc />
    public partial class groupMasterCreator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublishedToGroupIds",
                table: "Quizes");

            migrationBuilder.AddColumn<string>(
                name: "MasterId",
                table: "UsersGroups",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PublishedToQuizzes",
                columns: table => new
                {
                    PublishedToGroupId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuizzesId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishedToQuizzes", x => new { x.PublishedToGroupId, x.QuizzesId });
                    table.ForeignKey(
                        name: "FK_PublishedToQuizzes_Quizes_QuizzesId",
                        column: x => x.QuizzesId,
                        principalTable: "Quizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublishedToQuizzes_UsersGroups_PublishedToGroupId",
                        column: x => x.PublishedToGroupId,
                        principalTable: "UsersGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersGroups_MasterId",
                table: "UsersGroups",
                column: "MasterId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedToQuizzes_QuizzesId",
                table: "PublishedToQuizzes",
                column: "QuizzesId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersGroups_AspNetUsers_MasterId",
                table: "UsersGroups",
                column: "MasterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersGroups_AspNetUsers_MasterId",
                table: "UsersGroups");

            migrationBuilder.DropTable(
                name: "PublishedToQuizzes");

            migrationBuilder.DropIndex(
                name: "IX_UsersGroups_MasterId",
                table: "UsersGroups");

            migrationBuilder.DropColumn(
                name: "MasterId",
                table: "UsersGroups");

            migrationBuilder.AddColumn<string>(
                name: "PublishedToGroupIds",
                table: "Quizes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
