using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Main.Migrations
{
    /// <inheritdoc />
    public partial class RemovedRolesQuizzesSharedToUsersNotMails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublishedToEmails",
                table: "Quizes");

            migrationBuilder.CreateTable(
                name: "PublishedToUsers",
                columns: table => new
                {
                    PublishedToUsersId = table.Column<string>(type: "TEXT", nullable: false),
                    QuizSharedToUserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishedToUsers", x => new { x.PublishedToUsersId, x.QuizSharedToUserId });
                    table.ForeignKey(
                        name: "FK_PublishedToUsers_AspNetUsers_PublishedToUsersId",
                        column: x => x.PublishedToUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublishedToUsers_Quizes_QuizSharedToUserId",
                        column: x => x.QuizSharedToUserId,
                        principalTable: "Quizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PublishedToUsers_QuizSharedToUserId",
                table: "PublishedToUsers",
                column: "QuizSharedToUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PublishedToUsers");

            migrationBuilder.AddColumn<string>(
                name: "PublishedToEmails",
                table: "Quizes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
